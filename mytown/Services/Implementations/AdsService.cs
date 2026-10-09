using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Stripe;

namespace mytown.Services.Implementations
{
    // Message is safe to show to the user (returned as HTTP 400)
    public class AdsException : Exception
    {
        public AdsException(string message) : base(message) { }
    }

    public class AdsService : IAdsService
    {
        private const decimal FeeRate = 0.05m;
        private const decimal GstRate = 0.18m;
        private const decimal PricePerDay = 500m;
        private static readonly HashSet<string> ValidTypes = new() { "offer", "product", "video", "store" };

        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AdsService> _logger;

        public AdsService(AppDbContext db, IConfiguration config, ILogger<AdsService> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        // Accepts both naming styles found in your config files
        private string KeyId => (_config["Razorpay:KeyId"] ?? _config["Razorpay:Key"] ?? "").Trim();
        private string KeySecret => (_config["Razorpay:KeySecret"] ?? _config["Razorpay:Secret"] ?? "").Trim();

        // ---------------- helpers ----------------

        private static (decimal Budget, decimal Fee, decimal Gst, decimal Total) Price(decimal budget)
        {
            budget = Math.Round(budget, 2);
            var fee = Math.Round(budget * FeeRate, 2, MidpointRounding.AwayFromZero);
            var gst = Math.Round((budget + fee) * GstRate, 2, MidpointRounding.AwayFromZero);
            return (budget, fee, gst, budget + fee + gst);
        }

        private async Task EnsureBusinessAsync(int busRegId)
        {
            if (busRegId <= 0 || !await _db.BusinessRegisters.AnyAsync(b => b.BusRegId == busRegId))
                throw new AdsException("Business not found.");
        }

        private static string? CleanUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            url = url.Trim();
            if (url.Length > 1000) return null;
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? url : null;
        }

        private static JsonElement? ParseJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.Clone();
            }
            catch { return null; }
        }

        private static object ToDto(AdPromotion p)
        {
            var status = p.Status;
            var now = DateTime.UtcNow;
            if (status == "Active" && p.EndDate < now) status = "Expired";
            else if (status == "Active" && p.StartDate > now) status = "Scheduled";

            return new
            {
                id = p.PromotionId,
                busRegId = p.BusRegId,
                status,
                type = p.Type,
                title = p.Title,
                thumbnail = p.Thumbnail,
                mediaUrl = p.MediaUrl,
                placement = p.Placement,
                durationDays = p.DurationDays,
                budget = p.Budget,
                fee = p.Fee,
                gst = p.Gst,
                total = p.Total,
                startDate = p.StartDate,
                endDate = p.EndDate,
                transactionId = p.TransactionId,
                reviewNote = p.ReviewNote,
                createdAt = p.CreatedAt,
                content = ParseJson(p.ContentJson),
                audience = ParseJson(p.AudienceJson)
            };
        }

        // ---------------- promotions ----------------

        public async Task<object?> GetPromotionsAsync(int busRegId)
        {
            var list = await _db.AdPromotions
                .AsNoTracking()
                .Where(p => p.BusRegId == busRegId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(500)
                .ToListAsync();

            return list.Select(ToDto).ToList();
        }

        public async Task<object?> CreatePromotionAsync(AdsCreatePromotionDto dto)
        {
            await EnsureBusinessAsync(dto.BusRegId);

            var type = (dto.Type ?? "").Trim().ToLowerInvariant();
            if (!ValidTypes.Contains(type))
                throw new AdsException("Invalid promotion type.");

            var title = (dto.Title ?? "").Trim();
            if (title.Length == 0) title = "Promotion";
            if (title.Length > 200) title = title.Substring(0, 200);

            var isDraft = string.Equals(dto.Status?.Trim(), "Draft", StringComparison.OrdinalIgnoreCase);

            var start = (dto.StartDate ?? DateTime.UtcNow).ToUniversalTime();
            var days = Math.Clamp(dto.DurationDays, 1, 365);
            var end = (dto.EndDate ?? start.AddDays(days)).ToUniversalTime();
            if (end < start) throw new AdsException("End date must be after the start date.");

            var contentJson = dto.Content?.GetRawText();
            var audienceJson = dto.Audience?.GetRawText();
            if ((contentJson?.Length ?? 0) > 50000 || (audienceJson?.Length ?? 0) > 50000)
                throw new AdsException("Promotion content is too large.");

            var placement = (dto.Placement ?? "profile").Trim();
            if (placement.Length == 0 || placement.Length > 30) placement = "profile";

            var promo = new AdPromotion
            {
                BusRegId = dto.BusRegId,
                Type = type,
                Title = title,
                ContentJson = contentJson,
                AudienceJson = audienceJson,
                Thumbnail = CleanUrl(dto.Thumbnail),
                MediaUrl = CleanUrl(dto.MediaUrl),
                Placement = placement,
                DurationDays = days,
                StartDate = start,
                EndDate = end,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (isDraft)
            {
                if (dto.Id.HasValue)
                {
                    var existing = await _db.AdPromotions.FirstOrDefaultAsync(p =>
                        p.PromotionId == dto.Id && p.BusRegId == dto.BusRegId && p.Status == "Draft");
                    if (existing == null) throw new AdsException("Draft not found.");

                    existing.Type = type;
                    existing.Title = title;
                    existing.ContentJson = contentJson;
                    existing.AudienceJson = audienceJson;
                    existing.Thumbnail = promo.Thumbnail;
                    existing.MediaUrl = promo.MediaUrl;
                    existing.Placement = placement;
                    existing.DurationDays = days;
                    existing.StartDate = start;
                    existing.EndDate = end;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    return ToDto(existing);
                }
                promo.Status = "Draft";
            }
            else
            {
                if (dto.OrderId == null)
                    throw new AdsException("Payment is required before publishing.");

                var order = await _db.AdPaymentOrders.FirstOrDefaultAsync(o =>
                    o.AdPaymentOrderId == dto.OrderId && o.BusRegId == dto.BusRegId);

                if (order == null || order.Status != "Paid")
                    throw new AdsException("Payment not found or not completed.");

                if (await _db.AdPromotions.AnyAsync(p => p.PaymentOrderId == order.AdPaymentOrderId))
                    throw new AdsException("This payment has already been used for a promotion.");
                if (order.Budget != days * PricePerDay)
                    throw new AdsException("Paid amount does not match the selected duration.");
                if ((end - start).TotalDays > days + 1)
                    throw new AdsException("Promotion dates are longer than the paid duration.");

                promo.Status = "Pending";
                promo.PaymentOrderId = order.AdPaymentOrderId;
                promo.TransactionId = order.RazorpayPaymentId ?? order.StripePaymentIntentId;
                promo.Budget = order.Budget;
                promo.Fee = order.Fee;
                promo.Gst = order.Gst;
                promo.Total = order.Total;
            }

            _db.AdPromotions.Add(promo);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) when (!isDraft)
            {
                throw new AdsException("This payment has already been used for a promotion.");
            }

            return ToDto(promo);
        }

        // ---------------- media upload ----------------

        public async Task<object?> UploadMediaAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new AdsException("No file uploaded.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            const long mb = 1024 * 1024;
            var allowed = new Dictionary<string, (string ContentType, long Max)>
            {
                [".jpg"] = ("image/jpeg", 5 * mb),
                [".jpeg"] = ("image/jpeg", 5 * mb),
                [".png"] = ("image/png", 5 * mb),
                [".webp"] = ("image/webp", 5 * mb),
                [".mp4"] = ("video/mp4", 50 * mb),
                [".mov"] = ("video/quicktime", 50 * mb)
            };

            if (!allowed.TryGetValue(ext, out var rule))
                throw new AdsException("Only JPG, PNG, WebP images or MP4/MOV videos are allowed.");
            if (file.Length > rule.Max)
                throw new AdsException($"File must be under {rule.Max / mb} MB.");

            var conn = _config["AzureBlobStorage:ConnectionString"];
            var container = _config["AzureBlobStorage:ContainerName"];
            var blob = new BlobServiceClient(conn)
                .GetBlobContainerClient(container)
                .GetBlobClient($"ad_{Guid.NewGuid():N}{ext}");

            await using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = rule.ContentType }
            });

            return new { url = blob.Uri.ToString() };
        }

        // ---------------- payment ----------------

        public async Task<object?> CreatePaymentOrderAsync(AdsPaymentOrderRequestDto dto)
        {
            await EnsureBusinessAsync(dto.BusRegId);

            if (dto.DurationDays < 1 || dto.DurationDays > 365)
                throw new AdsException("Duration must be between 1 and 365 days.");
            if (KeyId.Length == 0 || KeySecret.Length == 0)
                throw new AdsException("Payments are not configured on the server.");

            var price = Price(dto.DurationDays * PricePerDay);
            var amountPaise = Convert.ToInt32(Math.Round(price.Total * 100m));

            string razorpayOrderId;
            try
            {
                var client = new Razorpay.Api.RazorpayClient(KeyId, KeySecret);
                var options = new Dictionary<string, object>
                {
                    { "amount", amountPaise },
                    { "currency", "INR" },
                    { "receipt", $"ad_{dto.BusRegId}_{DateTime.UtcNow:yyMMddHHmmss}" },
                    { "payment_capture", 1 }
                };
                Razorpay.Api.Order rzpOrder = client.Order.Create(options);
                razorpayOrderId = Convert.ToString(rzpOrder["id"]) ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Razorpay order creation failed for business {BusRegId}", dto.BusRegId);
                throw new AdsException("Could not start the payment. Please try again.");
            }

            if (razorpayOrderId.Length == 0)
                throw new AdsException("Could not start the payment. Please try again.");

            var entity = new AdPaymentOrder
            {
                BusRegId = dto.BusRegId,
                Budget = price.Budget,
                Fee = price.Fee,
                Gst = price.Gst,
                Total = price.Total,
                RazorpayOrderId = razorpayOrderId,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };
            _db.AdPaymentOrders.Add(entity);
            await _db.SaveChangesAsync();

            return new
            {
                orderId = entity.AdPaymentOrderId,
                razorpayOrderId,
                amount = amountPaise,
                currency = "INR",
                keyId = KeyId,
                budget = price.Budget,
                fee = price.Fee,
                gst = price.Gst,
                total = price.Total
            };
        }

        public async Task<object?> ConfirmPaymentAsync(AdsPaymentConfirmDto dto)
        {
            var order = await _db.AdPaymentOrders.FirstOrDefaultAsync(o =>
                o.AdPaymentOrderId == dto.OrderId && o.BusRegId == dto.BusRegId);

            if (order == null)
                throw new AdsException("Payment order not found.");
            if (!string.Equals(order.RazorpayOrderId, dto.RazorpayOrderId, StringComparison.Ordinal))
                throw new AdsException("Payment order mismatch.");

            // Already confirmed: allow the same payment again (safe retry), reject anything else
            if (order.Status == "Paid")
            {
                if (order.RazorpayPaymentId == dto.RazorpayPaymentId)
                    return new { transactionId = order.RazorpayPaymentId, status = "SUCCESS" };
                throw new AdsException("This order is already paid.");
            }

            if (KeySecret.Length == 0)
                throw new AdsException("Payments are not configured on the server.");

            // Razorpay signature = HMAC-SHA256(order_id|payment_id) with the key secret
            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            string expected;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(KeySecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                expected = Convert.ToHexString(hash).ToLowerInvariant();
            }

            var given = (dto.RazorpaySignature ?? "").Trim().ToLowerInvariant();
            if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(given)))
                throw new AdsException("Payment verification failed.");

            order.Status = "Paid";
            order.RazorpayPaymentId = dto.RazorpayPaymentId;
            order.PaidAt = DateTime.UtcNow;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new AdsException("This payment has already been used.");
            }

            return new { transactionId = order.RazorpayPaymentId, status = "SUCCESS" };
        }

        // ---------------- lookups for the wizard ----------------
                public async Task<object?> DeletePromotionAsync(int busRegId, int promotionId)
        {
            var p = await _db.AdPromotions.FirstOrDefaultAsync(x =>
                x.PromotionId == promotionId && x.BusRegId == busRegId);
            if (p == null) throw new AdsException("Promotion not found.");
            if (p.Status != "Draft")
                throw new AdsException("Only drafts can be deleted. Paid promotions cannot be removed.");

            _db.AdPromotions.Remove(p);
            await _db.SaveChangesAsync();
            return new { deleted = true };
        }

        public async Task<object?> GetProductsAsync(int busRegId)
        {
            var products = await _db.ProductsNew
                .AsNoTracking()
                .Where(p => p.BusRegId == busRegId && p.IsActive && p.ProductStatus == "ACTIVE")
                .OrderBy(p => p.ProductName)
                .Select(p => new { p.ProductId, p.ProductName })
                .ToListAsync();

            if (products.Count == 0) return new List<object>();

            var ids = products.Select(p => p.ProductId).ToList();

            var variants = await _db.ProductVariantsNew
                .AsNoTracking()
                .Where(v => ids.Contains(v.ProductId) && v.IsActive)
                .Select(v => new { v.SkuId, v.ProductId, v.Price, v.Discount, v.DiscountPrice, v.StockQuantity })
                .ToListAsync();

            var skuIds = variants.Select(v => v.SkuId).ToList();

            var images = await _db.ProductVariantImagesNew
                .AsNoTracking()
                .Where(i => skuIds.Contains(i.SkuId))
                .OrderBy(i => i.SortOrder).ThenBy(i => i.ImageId)
                .Select(i => new { i.SkuId, i.FileName })
                .ToListAsync();

            return products.Select(p =>
            {
                var vs = variants.Where(v => v.ProductId == p.ProductId).OrderBy(v => v.Price).ToList();
                var first = vs.FirstOrDefault();
                decimal price = first == null
                    ? 0
                    : (first.Discount > 0 && first.DiscountPrice.HasValue ? first.DiscountPrice.Value : first.Price);
                var stock = vs.Sum(v => v.StockQuantity);

                var image = "";
                foreach (var v in vs)
                {
                    image = images.FirstOrDefault(i => i.SkuId == v.SkuId)?.FileName ?? "";
                    if (image.Length > 0) break;
                }

                return new
                {
                    id = p.ProductId,
                    name = p.ProductName.Trim(),
                    price,
                    stock = (int)Math.Floor(stock),
                    image
                };
            }).ToList();
        }

                public async Task<object?> CreateStripeOrderAsync(AdsPaymentOrderRequestDto dto)
        {
            await EnsureBusinessAsync(dto.BusRegId);
            if (dto.DurationDays < 1 || dto.DurationDays > 365)
                throw new AdsException("Duration must be between 1 and 365 days.");

            var secret = _config["Stripe:SecretKey"]?.Trim();
            if (string.IsNullOrEmpty(secret))
                throw new AdsException("Card payments are not configured on the server.");

            var price = Price(dto.DurationDays * PricePerDay);
            var amountPaise = Convert.ToInt64(Math.Round(price.Total * 100m));

            var entity = new AdPaymentOrder
            {
                BusRegId = dto.BusRegId, Budget = price.Budget, Fee = price.Fee,
                Gst = price.Gst, Total = price.Total, Status = "Created",
                Provider = "Stripe", CreatedAt = DateTime.UtcNow
            };
            _db.AdPaymentOrders.Add(entity);
            await _db.SaveChangesAsync();

            PaymentIntent pi;
            try
            {
                StripeConfiguration.ApiKey = secret;
                pi = await new PaymentIntentService().CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = amountPaise,
                    Currency = "inr",
                    PaymentMethodTypes = new List<string> { "card" },
                    Description = $"Ad promotion order {entity.AdPaymentOrderId}",
                    Metadata = new Dictionary<string, string>
                    {
                        ["adOrderId"] = entity.AdPaymentOrderId.ToString(),
                        ["busRegId"] = dto.BusRegId.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe intent failed for business {BusRegId}", dto.BusRegId);
                throw new AdsException("Could not start the card payment. Please try again.");
            }

            entity.StripePaymentIntentId = pi.Id;
            await _db.SaveChangesAsync();

            return new
            {
                orderId = entity.AdPaymentOrderId,
                clientSecret = pi.ClientSecret,
                budget = price.Budget, fee = price.Fee, gst = price.Gst, total = price.Total
            };
        }

        public async Task<object?> ConfirmStripePaymentAsync(AdsStripeConfirmDto dto)
        {
            var order = await _db.AdPaymentOrders.FirstOrDefaultAsync(o =>
                o.AdPaymentOrderId == dto.OrderId && o.BusRegId == dto.BusRegId && o.Provider == "Stripe");
            if (order == null) throw new AdsException("Payment order not found.");

            if (order.Status == "Paid")
            {
                if (order.StripePaymentIntentId == dto.PaymentIntentId)
                    return new { transactionId = order.StripePaymentIntentId, status = "SUCCESS" };
                throw new AdsException("This order is already paid.");
            }
            if (order.StripePaymentIntentId != dto.PaymentIntentId)
                throw new AdsException("Payment order mismatch.");

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"]?.Trim();
            var pi = await new PaymentIntentService().GetAsync(dto.PaymentIntentId);

            if (pi.Status != "succeeded") throw new AdsException("Payment was not completed.");
            if (pi.AmountReceived != Convert.ToInt64(Math.Round(order.Total * 100m)))
                throw new AdsException("Paid amount does not match the order.");

            order.Status = "Paid";
            order.PaidAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return new { transactionId = pi.Id, status = "SUCCESS" };
        }

        public async Task<object?> GetStoreInfoAsync(int busRegId)
        {
            return await _db.BusinessRegisters
                .AsNoTracking()
                .Where(b => b.BusRegId == busRegId)
                .Select(b => new
                {
                    name = b.BusinessName,
                    location = b.Town + ", " + b.BusinessCity + ", " + b.BusinessState,
                    category = b.BusinessCategory.BusinessCategoryName,
                    email = b.BusEmail,
                    phone = b.BusMobileNo
                })
                .FirstOrDefaultAsync();
        }
    }
}