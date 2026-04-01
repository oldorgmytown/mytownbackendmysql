using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;

namespace mytown.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public PaymentRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Order> GetOrderWithShippingDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.ShippingDetails)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Payments> AddPaymentAsync(int orderId, decimal amountPaid, string paymentMethod, string stripePaymentIntentId)
        {
            var payment = new Payments
            {
                OrderId = orderId,
                AmountPaid = amountPaid,
                PaymentMethod = paymentMethod,
                StripePaymentIntentId = stripePaymentIntentId,
                PaymentStatus = "Paid",
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId)
        {
            var storeDetails = _context.OrderDetails
                                       .Where(od => od.OrderId == orderId)
                                       .Select(od => new BusinessRegisterDto
                                       {
                                           BusRegId = od.Store.BusRegId,
                                           Businessname = od.Store.BusinessName,
                                           BusinessUsername = od.Store.BusinessUsername,
                                           BusEmail = od.Store.BusEmail,
                                           BusMobileNo = od.Store.BusMobileNo
                                       })
                                       .Distinct()
                                       .ToList();

            return storeDetails;
        }

        public ShopperRegisterDto GetShopperDetailsByOrderId(int orderId)
        {
            var shopperDetails = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new ShopperRegisterDto
                {
                    ShopperRegId = o.ShopperRegister.ShopperRegId,
                    Username = o.ShopperRegister.Username,
                    Email = o.ShopperRegister.Email,
                    IsEmailVerified = o.ShopperRegister.IsEmailVerified,
                    Address = o.ShopperRegister.Address,
                    Town = o.ShopperRegister.Town,
                    City = o.ShopperRegister.City,
                    State = o.ShopperRegister.State,
                    Country = o.ShopperRegister.Country,
                    PostalCode = o.ShopperRegister.PostalCode,
                    PhoneNumber = o.ShopperRegister.PhoneNumber,
                    PhotoName = o.ShopperRegister.PhotoName,
                    ShopperRegDate = o.ShopperRegister.ShopperRegDate
                })
                .FirstOrDefault();

            return shopperDetails;
        }

        public List<ShippingDetails> GetShippingDetailsByOrderId(int orderId)
        {
            return _context.ShippingDetails
                .Where(s => s.OrderId == orderId)
                .Include(s => s.StoreOrder)
                .ToList();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SendEmailToCourier(int branchId, int storeOrderId)
        {
            // 1️⃣ Get courier info via branch
            var courierInfo = await _context.CourierBranches
                .Where(cb => cb.BranchId == branchId)
                .Select(cb => new
                {
                    cb.CourierServiceName,
                    CourierEmail = cb.CourierService.CourierEmail
                })
                .FirstOrDefaultAsync();

            if (courierInfo == null || string.IsNullOrEmpty(courierInfo.CourierEmail))
                return;

            // 2️⃣ Get store order info
            var storeOrderInfo = await _context.StoreOrders
                .Where(so => so.StoreOrderId == storeOrderId)
                .Select(so => new
                {
                    so.StoreOrderId,
                    StoreName = so.Store.BusinessName
                })
                .FirstOrDefaultAsync();

            if (storeOrderInfo == null)
                return;

            // 3️⃣ Get products for this store order
            var products = await _context.OrderDetails
                .Where(od => od.StoreOrderId == storeOrderId)
                .Select(od => new
                {
                    od.Product.ProductName,
                    od.Quantity
                })
                .ToListAsync();

            var productList = products
                .Select(p => (p.ProductName, p.Quantity))
                .ToList();

            // 4️⃣ Send email (ONE email per store order)
            await _emailService.SendEmailToCourierAsync(
                courierInfo.CourierEmail,
                courierInfo.CourierServiceName,
                storeOrderInfo.StoreOrderId,
                storeOrderInfo.StoreName,
                productList
            );
        }

        public async Task<int> GetCourierIdByBranchIdAsync(int branchId)
        {
            return await _context.CourierBranches
                .Where(b => b.BranchId == branchId)
                .Select(b => b.CourierId)
                .FirstAsync();
        }

        public async Task AddCourierNotificationAsync(CourierDBNotifications notification)
        {
            await _context.CourierDBNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }


        // add transporter notifications
        public async Task AddTransporterNotificationAsync(TransporterDBNotifications notification)
        {
            await _context.TransporterDBNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
        public List<(string ProductName, int Quantity)> GetProductsByStoreOrderId(int storeOrderId)
        {
            return _context.OrderDetails
                .Where(od => od.StoreOrderId == storeOrderId)
                .Select(od => new
                {
                    od.Product.ProductName,
                    od.Quantity
                })
                .AsEnumerable()
                .Select(p => (p.ProductName, p.Quantity))
                .ToList();
        }

        // ============================================================
        // P2P Delivery Implementations
        // ============================================================

        public async Task<StoreOrder?> GetStoreOrderByIdAsync(int storeOrderId)
        {
            return await _context.StoreOrders
                .Include(so => so.Order)
                .FirstOrDefaultAsync(so => so.StoreOrderId == storeOrderId);
        }

        public async Task CreateP2PDeliveryRequestAsync(CreateP2PDeliveryRequestDto dto)
        {
            // Avoid duplicates on retry
            var existing = await _context.TransporterDeliveryRequests
                .AnyAsync(d =>
                    d.PlanId == dto.PlanId &&
                    d.ShopperRegId == dto.ShopperRegId &&
                    d.OrderId == dto.OrderId);

            if (existing) return;

            var request = new TransporterDeliveryRequest
            {
                PlanId = dto.PlanId,
                TransporterRegId = dto.TransporterRegId,
                ShopperRegId = dto.ShopperRegId,
                OrderId = dto.OrderId,
                PickupLocation = dto.PickupLocation,
                DropoffLocation = dto.DropoffLocation,
                PackageWeightKg = dto.PackageWeightKg,
                NumberOfPackages = dto.NumberOfPackages,
                DeliveryFee = dto.DeliveryFee,
                PackageTags = dto.PackageTags ?? "NA",
                DeliveryStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                DeliveryProofFile = "" 
            };

            _context.TransporterDeliveryRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTransporterNotificationAsync(
            int transporterRegId,
            string title,
            string message)
        {
            // The TransporterDeliveryRequest row with status "Pending" acts as
            // the notification — transporter sees pending requests on their dashboard.
            // Add a dedicated transporter notifications table here later if needed.
            await Task.CompletedTask;
        }

        public async Task<string> GetStoreAddressAsync(int storeId)
        {
            return await _context.BusinessRegisters
                .Where(b => b.BusRegId == storeId)
                .Select(b => b.Address1 + ", " + b.Town + ", " + b.BusinessCity +
                             ", " + b.BusinessState + ", " + b.BusinessCountry)
                .FirstOrDefaultAsync() ?? "Store Address";
        }

        public async Task<decimal> GetStoreOrderWeightAsync(int storeOrderId)
        {
            return await (
                from od in _context.OrderDetails
                join sku in _context.Sku_ProductVariants on od.SkuId equals sku.SkuId
                where od.StoreOrderId == storeOrderId
                select (sku.Weight ?? 0) * od.Quantity
            ).SumAsync();
        }

        public async Task<int> GetStoreOrderItemCountAsync(int storeOrderId)
        {
            return await _context.OrderDetails
                .Where(od => od.StoreOrderId == storeOrderId)
                .SumAsync(od => od.Quantity);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}