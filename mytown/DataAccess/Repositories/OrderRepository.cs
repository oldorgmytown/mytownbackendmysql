using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using Stripe;
using static System.Collections.Specialized.BitVector32;

namespace mytown.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public OrderRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

//        public async Task<int> CreateOrderAsync(CreateOrderRequestddto request)
//        {
//            List<dynamic> items;

//            // 1️⃣ GET ITEMS (Cart OR BuyNow)
//            if (request.UseCart)
//            {
//                if (!request.ShopperRegId.HasValue)
//                    throw new Exception("ShopperId is required for cart checkout.");

//                var cartItems = await _context.addtocart
//                    .Where(c => c.ShopperRegId == request.ShopperRegId &&
//                                c.orderstatus == "Cart")
//                    .ToListAsync();

//                if (!cartItems.Any())
//                    return 0;

//                var productIds = cartItems
//                    .Select(c => (long)c.ProductId)
//                    .Distinct()
//                    .ToList();

//                var products = await _context.ProductsNew
//                    .Where(p => productIds.Contains(p.ProductId))
//                    .ToListAsync();

//                items = cartItems.Select(c =>
//                {
//                    var product = products.FirstOrDefault(p =>
//                        p.ProductId == (long)c.ProductId);

//                    if (product == null)
//                        throw new Exception(
//                            $"Product {c.ProductId} not found.");

//                    return new
//                    {
//                        ProductId = (long)c.ProductId,
//                        SkuId = (long)c.SkuId,
//                        BusRegId = product.BusRegId,
//                        Quantity = c.ProdQty,
//                        Price = c.ProductPrice
//                    };
//                }).ToList<dynamic>();
//            }
//            else
//            {
//                if (request.Items == null || !request.Items.Any())
//                    throw new Exception("Items required for Buy Now");

//                var productIds = request.Items.Select(i => (long)i.ProductId).Distinct().ToList();

//                var products = await (
//                    from p in _context.ProductsNew
//                    join bp in _context.BusinessProfiles
//                        on p.BusRegId equals bp.BusRegId
//                    where productIds.Contains(p.ProductId)
//                          && p.ProductStatus == "ACTIVE"
//                          && p.IsActive
//                          && bp.ProfileStatus == "approved"
//                    select p
//                ).ToListAsync();

//                if (products.Count != productIds.Count)
//                    throw new Exception("One or more products are no longer available.");

//                items = request.Items.Select(i =>
//                {
//                    var product = products.FirstOrDefault(p =>
//                        p.ProductId == i.ProductId);

//                    if (product == null)
//                        throw new Exception($"Product not found: {i.ProductId}");

//                    return new
//                    {
//                        ProductId = product.ProductId,
//                        SkuId = i.SkuId,
//                        BusRegId = product.BusRegId,
//                        Quantity = i.Quantity <= 0 ? 1 : i.Quantity,
//                        Price = i.Price
//                    };
//                }).ToList<dynamic>();
//            }

//            // 2️⃣ VALIDATION
//            var productIdsList = items.Select(c => (long)c.ProductId).Distinct().ToList();

//            var invalidItems = await (
//                from p in _context.ProductsNew
//                join bp in _context.BusinessProfiles
//                    on p.BusRegId equals bp.BusRegId
//                where productIdsList.Contains(p.ProductId)
//                      && (
//                            p.ProductStatus != "ACTIVE"
//                            || !p.IsActive
//                            || bp.ProfileStatus != "approved"
//                         )
//                select p.ProductId
//            ).ToListAsync();

//            if (invalidItems.Any())
//                throw new Exception("One or more products are no longer available.");

//            // 3️⃣ CREATE ORDER
//            decimal totalAmount = items.Sum(c => (decimal)c.Price * (int)c.Quantity);

//            var selectedAltAddressId = request.SelectedAltAddressId;
//            if (selectedAltAddressId.HasValue && selectedAltAddressId.Value <= 0)
//                selectedAltAddressId = null;

//            var newOrder = new Order
//            {
//                ShopperRegId = request.ShopperRegId,
//                GuestRegId = request.GuestRegId,
//                IsGuestOrder = request.IsGuestOrder,
//                SelectedAltAddressId = selectedAltAddressId,
//                TotalAmount = totalAmount,
//                ShippingType = "Multiple",
//                OrderStatus = "Pending",
//                OrderDate = DateTime.UtcNow
//            };

//            _context.Orders.Add(newOrder);
//            await _context.SaveChangesAsync();

//            // 4️⃣ ADDRESS
//            string deliveryAddress;

//            if (!request.IsGuestOrder)
//            {
//                if (selectedAltAddressId.HasValue)
//                {
//                    deliveryAddress = await _context.ShopperAlternateAddresses
//                        .Where(a => a.AltAddressId == selectedAltAddressId.Value)
//                        .Select(a =>
//                            a.AltName + ", " +
//                            a.AltAddress + ", " +
//                            a.AltTown + ", " +
//                            a.AltCity + ", " +
//                            a.AltState + ", " +
//                            a.AltCountry +
//                            (a.AltPostalCode != null ? " - " + a.AltPostalCode : "")
//                        )
//                        .FirstOrDefaultAsync();
//                }
//                else
//                {
//                    deliveryAddress = await _context.ShopperRegisters
//                        .Where(s => s.ShopperRegId == request.ShopperRegId)
//                        .Select(s =>
//                            s.Address + ", " +
//                            s.Town + ", " +
//                            s.City + ", " +
//                            s.State + ", " +
//                            s.Country +
//                            (s.PostalCode != null ? " - " + s.PostalCode : "")
//                        )
//                        .FirstOrDefaultAsync();
//                }
//            }
//            else
//            {
//                deliveryAddress = await _context.GuestRegisters
//                    .Where(g => g.GuestRegId == request.GuestRegId)
//                    .Select(g =>
//                        g.Address + ", " +
//                        g.Town + ", " +
//                        g.City + ", " +
//                        g.State + ", " +
//                        g.Country +
//                        (g.PostalCode != null ? " - " + g.PostalCode : "")
//                    )
//                    .FirstOrDefaultAsync();
//            }

//            // 5️⃣ STORE ORDERS
//            var groupedStores = items.GroupBy(c => (int)c.BusRegId);
//            var storeOrders = new List<StoreOrder>();

//            var shippingMap = request.ShippingSelections.ToDictionary(s => s.StoreId);

//            foreach (var group in groupedStores)
//            {
//                if (!shippingMap.TryGetValue(group.Key, out var shipping))
//                    throw new Exception($"Shipping selection missing for StoreId {group.Key}");

//                storeOrders.Add(new StoreOrder
//                {
//                    OrderId = newOrder.OrderId,
//                    StoreId = group.Key,
//                    StoreTotalAmount = group.Sum(x => (decimal)x.Price * (int)x.Quantity),
//                    Storeorder_Status = "Pending",
//                    CourierType = shipping.ShippingType
//                });
//            }

//            _context.StoreOrders.AddRange(storeOrders);
//            await _context.SaveChangesAsync();

//            var storeOrderMap = storeOrders.ToDictionary(s => (long) s.StoreId, s => s.StoreOrderId);

//            // 6️⃣ NOTIFICATIONS
//            var notifications = storeOrders.Select(so => new BusinessDBNotifications
//            {
//                BusRegId = so.StoreId,
//                Title = "New Order Received",
//                Message = $"Store Order #{so.StoreOrderId} has been placed",
//                IsRead = false,
//                CreatedDate = DateTime.UtcNow
//            }).ToList();

//            _context.BusinessDBNotifications.AddRange(notifications);
//            await _context.SaveChangesAsync();

//            // 7️⃣ ORDER DETAILS
//            var orderDetailsList = items.Select(item => new orderdetails
//            {
//                OrderId = newOrder.OrderId,
//                ProductId = item.ProductId,
//                SkuId = item.SkuId,
//                StoreId = item.BusRegId,
//                Quantity = item.Quantity,
//                Price = item.Price,
//                StoreOrderId = storeOrderMap[item.BusRegId]
//            }).ToList();

//            _context.OrderDetails.AddRange(orderDetailsList);
//            await _context.SaveChangesAsync();

//            // 8️⃣ SHIPPING
//            var shippingList = new List<ShippingDetails>();

//            foreach (var storeOrder in storeOrders)
//            {
//                var shippingSelection = request.ShippingSelections
//                    .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

//                if (shippingSelection == null)
//                    throw new Exception($"Shipping missing for StoreId {storeOrder.StoreId}");

//                // ─── PICKUP PATH ───────────────────────────────────────────────
//                if (shippingSelection.ShippingType?.Trim().ToLower() == "pickup")
//                {
//                    var pickupStore = await _context.BusinessRegisters
//                        .AsNoTracking()
//                        .FirstOrDefaultAsync(b => b.BusRegId == storeOrder.StoreId);

//                    if (pickupStore == null)
//                        throw new Exception($"Store not found: {storeOrder.StoreId}");

//                    // TODO: if you add a flag like PickupEnabled on the store, validate it here
//                    // if (!pickupStore.PickupEnabled) throw new Exception("Pickup not available for this store.");

//                    shippingList.Add(new ShippingDetails
//                    {
//                        OrderId = newOrder.OrderId,
//                        StoreOrderId = storeOrder.StoreOrderId,
//                        BranchId = null,
//                        TransporterRegId = null,
//                        TransporterPlanId = null,
//                        ShippingType = "Pickup",
//                        EstimatedDays = 0,
//                        Cost = 0m,                      // forced on the backend, never from the request
//                        TrackingId = "",
//                        ShippingStatus = "Pending",
//                        DeliveryAddress = deliveryAddress
//                    });

//                    continue;   // skip P2P / courier logic
//                }

//                // ─── P2P PATH ──────────────────────────────────────────────────────────
//                if (shippingSelection.ShippingType?.Trim().ToLower() == "p2p")
//                {
//                    // Validate transporter info was passed
//                    if (!shippingSelection.TransporterRegId.HasValue || shippingSelection.TransporterRegId <= 0)
//                        throw new Exception($"TransporterRegId required for P2P shipping (StoreId {storeOrder.StoreId})");

//                    if (!shippingSelection.TransporterPlanId.HasValue || shippingSelection.TransporterPlanId <= 0)
//                        throw new Exception($"TransporterPlanId required for P2P shipping (StoreId {storeOrder.StoreId})");

//                    // Get the travel plan to get estimated days and cost
//                    var travelPlan = await _context.TransporterTravelPlans
//                        .FirstOrDefaultAsync(p =>
//                            p.PlanId == shippingSelection.TransporterPlanId.Value &&
//                            p.TransporterRegId == shippingSelection.TransporterRegId.Value &&
//                            p.IsActive);

//                    if (travelPlan == null)
//                        throw new Exception("Selected P2P transporter plan is no longer available.");

//                    int estimatedDays = Math.Max(1,
//                        (travelPlan.ArrivalDate.Date - travelPlan.StartDate.Date).Days);

//                    var store = await _context.BusinessRegisters
//     .FirstOrDefaultAsync(b => b.BusRegId == storeOrder.StoreId);

//                    string destinationCity = "";

//                    if (request.IsGuestOrder)
//                    {
//                        var guest = await _context.GuestRegisters
//                            .FirstOrDefaultAsync(g =>
//                                g.GuestRegId == request.GuestRegId);

//                        destinationCity = guest?.City ?? "";
//                    }
//                    else
//                    {
//                        var shopper = await _context.ShopperRegisters
//                            .FirstOrDefaultAsync(s =>
//                                s.ShopperRegId == request.ShopperRegId);

//                        destinationCity = shopper?.City ?? "";
//                    }

//                    decimal p2pCost = 50m; // default fallback

//                    if (store != null && !string.IsNullOrEmpty(destinationCity))
//                    {
//                        // Find cheapest surface service for this store → customer route
//                        var surfaceService = await (
//                            from branch in _context.CourierBranches
//                            join service in _context.CourierBranchServices
//                                on branch.BranchId equals service.BranchId
//                            where branch.City.ToLower() == store.BusinessCity.ToLower()
//                               && branch.State.ToLower() == store.BusinessState.ToLower()
//                               && branch.Country.ToLower() == store.BusinessCountry.ToLower()
//                               && service.ShippingMode == "Surface"
//                               && !string.IsNullOrEmpty(service.Destinations)
//                            select service
//                        )
//                        .AsNoTracking()
//                        .ToListAsync();

//                        var matchingSurface = surfaceService
//                            .Where(s => s.Destinations
//                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
//                                .Select(d => d.Trim().ToLower())
//                                .Contains(destinationCity.ToLower()))
//                            .OrderBy(s => s.Charges)
//                            .FirstOrDefault();

//                        if (matchingSurface != null)
//                            p2pCost = Math.Max(
//                                50m,
//                                Math.Round(matchingSurface.Charges * 0.30m, 2));
//                    }

//                    // Save ShippingDetails with BranchId = 1 (P2P has no courier branch)
//                    var p2pShipping = new ShippingDetails
//{
//    OrderId = newOrder.OrderId,
//    StoreOrderId = storeOrder.StoreOrderId,
//    BranchId = null,
//    ShippingType = "P2P",
//    EstimatedDays = estimatedDays,
//    Cost = p2pCost,
//    TrackingId = "",
//    ShippingStatus = "Pending",
//    DeliveryAddress = deliveryAddress,
//    TransporterRegId = shippingSelection.TransporterRegId,
//    TransporterPlanId = shippingSelection.TransporterPlanId 
//};

//                    shippingList.Add(p2pShipping);
//                }
//                else
//                {
//                    // ─── STANDARD / EXPRESS PATH (existing logic unchanged) ────────────
//                    var branch = await _context.CourierBranches
//                        .FirstOrDefaultAsync(b => b.BranchId == shippingSelection.BranchId);

//                    if (branch == null)
//                        throw new Exception($"Branch not found: {shippingSelection.BranchId}");

//                    string dbShippingMode = shippingSelection.ShippingType?.Trim().ToLower() switch
//                    {
//                        "standard" => "Surface",
//                        "express" => "Air",
//                        _ => throw new Exception($"Invalid shipping type: {shippingSelection.ShippingType}")
//                    };

//                    var service = await _context.CourierBranchServices
//                        .FirstOrDefaultAsync(s =>
//                            s.BranchId == shippingSelection.BranchId &&
//                            s.ShippingMode == dbShippingMode);

//                    if (service == null)
//                        throw new Exception("Courier service configuration not found.");

//                    var shipping = new ShippingDetails
//                    {
//                        OrderId = newOrder.OrderId,
//                        StoreOrderId = storeOrder.StoreOrderId,
//                        BranchId = branch.BranchId,
//                        ShippingType = shippingSelection.ShippingType?.Trim().ToLower() switch
//                        {
//                            "standard" => "Standard",
//                            "express" => "Express",
//                            _ => shippingSelection.ShippingType
//                        },
//                        EstimatedDays = service.EstimateDays ?? 0,
//                        Cost = service.Charges,
//                        TrackingId = "",
//                        ShippingStatus = "Pending",
//                        DeliveryAddress = deliveryAddress
//                    };

//                    shippingList.Add(shipping);
//                }
//            }

//            // Save storeOrders again (because we updated CourierType for P2P above)
//            await _context.SaveChangesAsync();

//            _context.ShippingDetails.AddRange(shippingList);
//            await _context.SaveChangesAsync();

//            return newOrder.OrderId;
//        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //For multiple transprter selection
        public async Task<int> CreateOrderAsync(CreateOrderRequestddto request)
        {
            // ✅ OPTIONAL (recommended): one transaction for the whole order
            // await using var tx = await _context.Database.BeginTransactionAsync();

            List<dynamic> items;

            // 1️⃣ GET ITEMS (Cart OR BuyNow)
            if (request.UseCart)
            {
                if (!request.ShopperRegId.HasValue)
                    throw new Exception("ShopperId is required for cart checkout.");

                var cartItems = await _context.addtocart
                    .Where(c => c.ShopperRegId == request.ShopperRegId &&
                                c.orderstatus == "Cart")
                    .ToListAsync();

                if (!cartItems.Any())
                    return 0;

                var productIds = cartItems
                    .Select(c => (long)c.ProductId)
                    .Distinct()
                    .ToList();

                var products = await _context.ProductsNew
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToListAsync();

                items = cartItems.Select(c =>
                {
                    var product = products.FirstOrDefault(p =>
                        p.ProductId == (long)c.ProductId);

                    if (product == null)
                        throw new Exception(
                            $"Product {c.ProductId} not found.");

                    return new
                    {
                        ProductId = (long)c.ProductId,
                        SkuId = (long)c.SkuId,
                        BusRegId = product.BusRegId,
                        Quantity = c.ProdQty,
                        Price = c.ProductPrice
                    };
                }).ToList<dynamic>();
            }
            else
            {
                if (request.Items == null || !request.Items.Any())
                    throw new Exception("Items required for Buy Now");

                var productIds = request.Items.Select(i => (long)i.ProductId).Distinct().ToList();

                var products = await (
                    from p in _context.ProductsNew
                    join bp in _context.BusinessProfiles
                        on p.BusRegId equals bp.BusRegId
                    where productIds.Contains(p.ProductId)
                          && p.ProductStatus == "ACTIVE"
                          && p.IsActive
                          && bp.ProfileStatus == "approved"
                    select p
                ).ToListAsync();

                if (products.Count != productIds.Count)
                    throw new Exception("One or more products are no longer available.");

                items = request.Items.Select(i =>
                {
                    var product = products.FirstOrDefault(p =>
                        p.ProductId == i.ProductId);

                    if (product == null)
                        throw new Exception($"Product not found: {i.ProductId}");

                    return new
                    {
                        ProductId = product.ProductId,
                        SkuId = i.SkuId,
                        BusRegId = product.BusRegId,
                        Quantity = i.Quantity <= 0 ? 1 : i.Quantity,
                        Price = i.Price
                    };
                }).ToList<dynamic>();
            }

            // 2️⃣ VALIDATION
            var productIdsList = items.Select(c => (long)c.ProductId).Distinct().ToList();

            var invalidItems = await (
                from p in _context.ProductsNew
                join bp in _context.BusinessProfiles
                    on p.BusRegId equals bp.BusRegId
                where productIdsList.Contains(p.ProductId)
                      && (
                            p.ProductStatus != "ACTIVE"
                            || !p.IsActive
                            || bp.ProfileStatus != "approved"
                         )
                select p.ProductId
            ).ToListAsync();

            if (invalidItems.Any())
                throw new Exception("One or more products are no longer available.");

            // 3️⃣ CREATE ORDER
            decimal totalAmount = items.Sum(c => (decimal)c.Price * (int)c.Quantity);

            var selectedAltAddressId = request.SelectedAltAddressId;
            if (selectedAltAddressId.HasValue && selectedAltAddressId.Value <= 0)
                selectedAltAddressId = null;

            var newOrder = new Order
            {
                ShopperRegId = request.ShopperRegId,
                GuestRegId = request.GuestRegId,
                IsGuestOrder = request.IsGuestOrder,
                SelectedAltAddressId = selectedAltAddressId,
                TotalAmount = totalAmount,
                ShippingType = "Multiple",
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // 4️⃣ ADDRESS
            string deliveryAddress;

            if (!request.IsGuestOrder)
            {
                if (selectedAltAddressId.HasValue)
                {
                    deliveryAddress = await _context.ShopperAlternateAddresses
                        .Where(a => a.AltAddressId == selectedAltAddressId.Value)
                        .Select(a =>
                            a.AltName + ", " +
                            a.AltAddress + ", " +
                            a.AltTown + ", " +
                            a.AltCity + ", " +
                            a.AltState + ", " +
                            a.AltCountry +
                            (a.AltPostalCode != null ? " - " + a.AltPostalCode : "")
                        )
                        .FirstOrDefaultAsync();
                }
                else
                {
                    deliveryAddress = await _context.ShopperRegisters
                        .Where(s => s.ShopperRegId == request.ShopperRegId)
                        .Select(s =>
                            s.Address + ", " +
                            s.Town + ", " +
                            s.City + ", " +
                            s.State + ", " +
                            s.Country +
                            (s.PostalCode != null ? " - " + s.PostalCode : "")
                        )
                        .FirstOrDefaultAsync();
                }
            }
            else
            {
                deliveryAddress = await _context.GuestRegisters
                    .Where(g => g.GuestRegId == request.GuestRegId)
                    .Select(g =>
                        g.Address + ", " +
                        g.Town + ", " +
                        g.City + ", " +
                        g.State + ", " +
                        g.Country +
                        (g.PostalCode != null ? " - " + g.PostalCode : "")
                    )
                    .FirstOrDefaultAsync();
            }

            // 5️⃣ STORE ORDERS
            var groupedStores = items.GroupBy(c => (int)c.BusRegId);
            var storeOrders = new List<StoreOrder>();

            var shippingMap = request.ShippingSelections.ToDictionary(s => s.StoreId);

            foreach (var group in groupedStores)
            {
                if (!shippingMap.TryGetValue(group.Key, out var shipping))
                    throw new Exception($"Shipping selection missing for StoreId {group.Key}");

                storeOrders.Add(new StoreOrder
                {
                    OrderId = newOrder.OrderId,
                    StoreId = group.Key,
                    StoreTotalAmount = group.Sum(x => (decimal)x.Price * (int)x.Quantity),
                    Storeorder_Status = "Pending",
                    CourierType = shipping.ShippingType
                });
            }

            _context.StoreOrders.AddRange(storeOrders);
            await _context.SaveChangesAsync();

            var storeOrderMap = storeOrders.ToDictionary(s => (long)s.StoreId, s => s.StoreOrderId);

            // 6️⃣ NOTIFICATIONS
            var notifications = storeOrders.Select(so => new BusinessDBNotifications
            {
                BusRegId = so.StoreId,
                Title = "New Order Received",
                Message = $"Store Order #{so.StoreOrderId} has been placed",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            _context.BusinessDBNotifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // 7️⃣ ORDER DETAILS
            var orderDetailsList = items.Select(item => new orderdetails
            {
                OrderId = newOrder.OrderId,
                ProductId = item.ProductId,
                SkuId = item.SkuId,
                StoreId = item.BusRegId,
                Quantity = item.Quantity,
                Price = item.Price,
                StoreOrderId = storeOrderMap[item.BusRegId]
            }).ToList();

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            // ✅ NEW (E2) ------------------------------------------------------------
            var bookingNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time");
            var p2pChains = new Dictionary<int, List<TransporterTravelPlan>>();   // StoreOrderId -> chain of plans
            // ✅ END NEW -------------------------------------------------------------

            // 8️⃣ SHIPPING
            var shippingList = new List<ShippingDetails>();

            foreach (var storeOrder in storeOrders)
            {
                var shippingSelection = request.ShippingSelections
                    .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

                if (shippingSelection == null)
                    throw new Exception($"Shipping missing for StoreId {storeOrder.StoreId}");

                // ─── PICKUP PATH ───────────────────────────────────────────────
                if (shippingSelection.ShippingType?.Trim().ToLower() == "pickup")
                {
                    var pickupStore = await _context.BusinessRegisters
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.BusRegId == storeOrder.StoreId);

                    if (pickupStore == null)
                        throw new Exception($"Store not found: {storeOrder.StoreId}");

                    // TODO: if you add a flag like PickupEnabled on the store, validate it here
                    // if (!pickupStore.PickupEnabled) throw new Exception("Pickup not available for this store.");

                    shippingList.Add(new ShippingDetails
                    {
                        OrderId = newOrder.OrderId,
                        StoreOrderId = storeOrder.StoreOrderId,
                        BranchId = null,
                        TransporterRegId = null,
                        TransporterPlanId = null,
                        ShippingType = "Pickup",
                        EstimatedDays = 0,
                        Cost = 0m,                      // forced on the backend, never from the request
                        TrackingId = "",
                        ShippingStatus = "Pending",
                        DeliveryAddress = deliveryAddress
                    });

                    continue;   // skip P2P / courier logic
                }

                // ─── P2P PATH ──────────────────────────────────────────────────────────
                if (shippingSelection.ShippingType?.Trim().ToLower() == "p2p")
                {
                    // Validate transporter info was passed
                    if (!shippingSelection.TransporterRegId.HasValue || shippingSelection.TransporterRegId <= 0)
                        throw new Exception($"TransporterRegId required for P2P shipping (StoreId {storeOrder.StoreId})");

                    if (!shippingSelection.TransporterPlanId.HasValue || shippingSelection.TransporterPlanId <= 0)
                        throw new Exception($"TransporterPlanId required for P2P shipping (StoreId {storeOrder.StoreId})");

                    // Get the travel plan to get estimated days and cost
                    var travelPlan = await _context.TransporterTravelPlans
                        .FirstOrDefaultAsync(p =>
                            p.PlanId == shippingSelection.TransporterPlanId.Value &&
                            p.TransporterRegId == shippingSelection.TransporterRegId.Value &&
                            p.IsActive);

                    if (travelPlan == null)
                        throw new Exception("Selected P2P transporter plan is no longer available.");

                    // ✅ NEW (E3) --------------------------------------------------------
                    // (this replaces the old "int estimatedDays = Math.Max(1, (travelPlan.ArrivalDate.Date - travelPlan.StartDate.Date).Days);")
                    var hops = shippingSelection.Hops;
                    List<TransporterTravelPlan> chain;

                    if (hops != null && hops.Count > 0)
                    {
                        chain = await ValidateChainAsync(hops, bookingNow);

                        // first plan must be the same one the app sent as TransporterPlanId
                        if (chain[0].PlanId != shippingSelection.TransporterPlanId.Value)
                            throw new Exception("Selected P2P route is no longer valid.");
                    }
                    else
                    {
                        chain = new List<TransporterTravelPlan> { travelPlan };   // single transporter, as today
                    }

                    p2pChains[storeOrder.StoreOrderId] = chain;

                    // days from booking to the final arrival (same as the matching result)
                    int estimatedDays = Math.Max(1, (chain.Last().ArrivalDate.Date - bookingNow.Date).Days);
                    // ✅ END NEW ---------------------------------------------------------

                    var store = await _context.BusinessRegisters
     .FirstOrDefaultAsync(b => b.BusRegId == storeOrder.StoreId);

                    string destinationCity = "";

                    if (request.IsGuestOrder)
                    {
                        var guest = await _context.GuestRegisters
                            .FirstOrDefaultAsync(g =>
                                g.GuestRegId == request.GuestRegId);

                        destinationCity = guest?.City ?? "";
                    }
                    else
                    {
                        var shopper = await _context.ShopperRegisters
                            .FirstOrDefaultAsync(s =>
                                s.ShopperRegId == request.ShopperRegId);

                        destinationCity = shopper?.City ?? "";
                    }

                    decimal p2pCost = 50m; // default fallback

                    if (store != null && !string.IsNullOrEmpty(destinationCity))
                    {
                        // Find cheapest surface service for this store → customer route
                        var surfaceService = await (
                            from branch in _context.CourierBranches
                            join service in _context.CourierBranchServices
                                on branch.BranchId equals service.BranchId
                            where branch.City.ToLower() == store.BusinessCity.ToLower()
                               && branch.State.ToLower() == store.BusinessState.ToLower()
                               && branch.Country.ToLower() == store.BusinessCountry.ToLower()
                               && service.ShippingMode == "Surface"
                               && !string.IsNullOrEmpty(service.Destinations)
                            select service
                        )
                        .AsNoTracking()
                        .ToListAsync();

                        var matchingSurface = surfaceService
                            .Where(s => s.Destinations
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(d => d.Trim().ToLower())
                                .Contains(destinationCity.ToLower()))
                            .OrderBy(s => s.Charges)
                            .FirstOrDefault();

                        if (matchingSurface != null)
                            p2pCost = Math.Max(
                                50m,
                                Math.Round(matchingSurface.Charges * 0.30m, 2));
                    }

                    // ✅ NEW (E4) --------------------------------------------------------
                    if (chain.Count > 1)
                        p2pCost = 50m * chain.Count;      // 100 for 2 hops, 150 for 3 hops, same as shown to the shopper
                    // ✅ END NEW ---------------------------------------------------------

                    // Save ShippingDetails with BranchId = 1 (P2P has no courier branch)
                    var p2pShipping = new ShippingDetails
                    {
                        OrderId = newOrder.OrderId,
                        StoreOrderId = storeOrder.StoreOrderId,
                        BranchId = null,
                        ShippingType = "P2P",
                        EstimatedDays = estimatedDays,
                        Cost = p2pCost,
                        TrackingId = "",
                        ShippingStatus = "Pending",
                        DeliveryAddress = deliveryAddress,
                        TransporterRegId = shippingSelection.TransporterRegId,
                        TransporterPlanId = shippingSelection.TransporterPlanId
                    };

                    shippingList.Add(p2pShipping);
                }
                else
                {
                    // ─── STANDARD / EXPRESS PATH (existing logic unchanged) ────────────
                    var branch = await _context.CourierBranches
                        .FirstOrDefaultAsync(b => b.BranchId == shippingSelection.BranchId);

                    if (branch == null)
                        throw new Exception($"Branch not found: {shippingSelection.BranchId}");

                    string dbShippingMode = shippingSelection.ShippingType?.Trim().ToLower() switch
                    {
                        "standard" => "Surface",
                        "express" => "Air",
                        _ => throw new Exception($"Invalid shipping type: {shippingSelection.ShippingType}")
                    };

                    var service = await _context.CourierBranchServices
                        .FirstOrDefaultAsync(s =>
                            s.BranchId == shippingSelection.BranchId &&
                            s.ShippingMode == dbShippingMode);

                    if (service == null)
                        throw new Exception("Courier service configuration not found.");

                    var shipping = new ShippingDetails
                    {
                        OrderId = newOrder.OrderId,
                        StoreOrderId = storeOrder.StoreOrderId,
                        BranchId = branch.BranchId,
                        ShippingType = shippingSelection.ShippingType?.Trim().ToLower() switch
                        {
                            "standard" => "Standard",
                            "express" => "Express",
                            _ => shippingSelection.ShippingType
                        },
                        EstimatedDays = service.EstimateDays ?? 0,
                        Cost = service.Charges,
                        TrackingId = "",
                        ShippingStatus = "Pending",
                        DeliveryAddress = deliveryAddress
                    };

                    shippingList.Add(shipping);
                }
            }

            // Save storeOrders again (because we updated CourierType for P2P above)
            await _context.SaveChangesAsync();

            _context.ShippingDetails.AddRange(shippingList);

            // ✅ NEW (E5) ------------------------------------------------------------
            // one set of leg rows for every P2P shipment (a single transporter gets one leg)
            foreach (var s in shippingList.Where(x => x.ShippingType == "P2P"))
                _context.OrderTransportLegs.AddRange(BuildLegs(s, p2pChains[s.StoreOrderId]));
            // ✅ END NEW -------------------------------------------------------------

            await _context.SaveChangesAsync();     // shipping rows and legs saved together

            // await tx.CommitAsync();              // ✅ OPTIONAL, only if you added the transaction at the top

            return newOrder.OrderId;
        }

        public async Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId)
        {
            // Step 1: Get all cart items for this shopper
            var cartItems = await (from cart in _context.addtocart
                                   join sku in _context.Sku_ProductVariants
                                   on cart.SkuId equals sku.SkuId
                                   where cart.ShopperRegId == shopperRegId
                                         && cart.orderstatus == "Cart"
                                   select new
                                   {
                                       cart,
                                       sku
                                   }).ToListAsync();

            if (!cartItems.Any())
                return 0;

            // Step 2: Calculate total order amount based on SKU cost
            decimal totalAmount = cartItems.Sum(c => (c.sku.Sku_Cost) * c.cart.ProdQty);

            // Step 3: Create a new order
            var newOrder = new Order
            {
                ShopperRegId = shopperRegId,
                TotalAmount = totalAmount,
                ShippingType = "Multiple",
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Step 4: Group cart items by Store (BusRegId)
            var itemsGroupedByStore = cartItems.GroupBy(x => x.cart.BusRegId);

            List<StoreOrder> createdStoreOrders = new();

            // Step 5: Create StoreOrder per store
            foreach (var storeGroup in itemsGroupedByStore)
            {
                int storeId = storeGroup.Key;

                var storeOrder = new StoreOrder
                {
                    OrderId = newOrder.OrderId,
                    StoreId = storeId,
                    Storeorder_Status = "Pending"
                };

                _context.StoreOrders.Add(storeOrder);
                await _context.SaveChangesAsync(); // Save to generate StoreOrderId

                createdStoreOrders.Add(storeOrder);

                // Step 6: Create OrderDetails linked to StoreOrderId
                List<orderdetails> detailList = storeGroup.Select(item => new orderdetails
                {
                    OrderId = newOrder.OrderId,
                    StoreOrderId = storeOrder.StoreOrderId,   // IMPORTANT
                    ProductId = item.cart.ProductId,
                    SkuId = item.cart.SkuId,
                    StoreId = storeId,
                    Quantity = item.cart.ProdQty,
                    Price = item.sku.Sku_Cost
                }).ToList();

                _context.OrderDetails.AddRange(detailList);
            }

            // Save all order details
            await _context.SaveChangesAsync();

            return newOrder.OrderId;
        }


        public async Task SaveShippingSelectionsAsync(
            int orderId,
            List<StoreShippingSelection> selections)
        {
            // 1️⃣ Load store orders
            var storeOrders = await _context.StoreOrders
                .Where(s => s.OrderId == orderId)
                .ToListAsync();

            if (!storeOrders.Any())
                throw new Exception("No store orders found for this order.");

            // 2️⃣ Get order date once
            var orderDate = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => o.OrderDate)
                .FirstAsync();

            var shippingList = new List<ShippingDetails>();

            foreach (var storeOrder in storeOrders)
            {
                // 3️⃣ Find selection for store
                var selection = selections
                    .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

                if (selection == null)
                    throw new Exception($"No shipping selection found for store ID {storeOrder.StoreId}");

                // 4️⃣ Fetch courier branch from DB
                var branch = await _context.CourierBranches
                    .FirstOrDefaultAsync(b => b.BranchId == selection.BranchId);

                if (branch == null)
                    throw new Exception("Invalid courier branch selected.");

                // ✅ FETCH SERVICE
                var service = await _context.CourierBranchServices
                    .FirstOrDefaultAsync(s =>
                        s.BranchId == selection.BranchId &&
                        s.ShippingMode == selection.ShippingType);

                if (service == null)
                    throw new Exception("Courier service configuration not found.");

                if (!service.EstimateDays.HasValue)
                    throw new Exception("Estimated delivery days not configured for this courier.");

                // 5️⃣ Create shipping record (PER STORE)
                var shipping = new ShippingDetails
                {
                    OrderId = orderId,
                    StoreOrderId = storeOrder.StoreOrderId,
                    BranchId = branch.BranchId,
                    ShippingType = selection.ShippingType,
                    EstimatedDays = service.EstimateDays.Value,
                    Cost = service.Charges,
                    TrackingId = null,
                    ShippingStatus = "Pending"
                };

                shippingList.Add(shipping);
            }

            // 6️⃣ Save all shipping records
            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();
        }


        public async Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId)
        {
            // 1️⃣ Order + Shopper basic details + PaymentMethod
                        var order = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.ShopperRegId,
                    o.GuestRegId,
                    o.IsGuestOrder,

                    ShopperName = o.IsGuestOrder
                        ? o.GuestRegister.Username
                        : o.ShopperRegister.Username,

                    ShopperEmail = o.IsGuestOrder
                        ? o.GuestRegister.Email
                        : o.ShopperRegister.Email,

                    ShopperPhone = o.IsGuestOrder
                        ? o.GuestRegister.PhoneNumber
                        : o.ShopperRegister.PhoneNumber,

                    LatestPayment = _context.Payments
                        .Where(p => p.OrderId == o.OrderId)
                        .OrderByDescending(p => p.PaymentDate)
                        .Select(p => new
                        {
                            p.PaymentId,
                            p.PaymentMethod,
                            p.AmountPaid
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return null;

            // 2️⃣ Store + Shipping + Business Email + DeliveryAddress
            var stores = await (
       from so in _context.StoreOrders

       join sd in _context.ShippingDetails
           on so.StoreOrderId equals sd.StoreOrderId

       join b in _context.BusinessRegisters
           on so.StoreId equals b.BusRegId

       // ✅ LEFT JOIN CourierBranches
       join br in _context.CourierBranches
           on sd.BranchId equals br.BranchId into brGroup
       from br in brGroup.DefaultIfEmpty()

           // ✅ LEFT JOIN CourierService
       join c in _context.CourierService
           on br.CourierId equals c.CourierId into cGroup
       from c in cGroup.DefaultIfEmpty()

           // ✅ LEFT JOIN TransporterRegisters
       join t in _context.TransporterRegisters
           on sd.TransporterRegId equals t.TransporterRegId into tGroup
       from t in tGroup.DefaultIfEmpty()

       where so.OrderId == orderId

       select new StoreOrderConfirmationDto
       {
           StoreOrderId = so.StoreOrderId,
           StoreId = so.StoreId,
           StoreName = b.BusinessName,
           BusinessEmail = b.BusEmail,
           StoreAddress = b.Address1 + ", " +
               (string.IsNullOrEmpty(b.Address2) ? "" : b.Address2 + ", ") +
               b.BusinessCity + ", " +
               b.BusinessState + ", " +
               b.BusinessCountry +
               (string.IsNullOrEmpty(b.PostalCode) ? "" : " - " + b.PostalCode),
           BusinessPhone = b.BusMobileNo,

           // ✅ Courier (only if exists)
           CourierName = c != null ? c.CourierServiceName : null,
           CourierEmail = br != null ? br.BranchEmailId : null,
           CourierPhone = br != null ? br.BranchPhoneNumber : null,

           // ✅ Transporter (only if exists)
           TransporterName = t != null ? t.TransporterName : null,
           TransporterEmail = t != null ? t.Email : null,
           TransporterPhone = t != null ? t.PhoneNumber : null,

           ShippingType = sd.ShippingType,
           ShippingAmount = sd.Cost,
           EstimatedDays = sd.EstimatedDays,
           EstimatedDeliveryDate = order.OrderDate.AddDays(sd.EstimatedDays),
           ShippingStatus = sd.ShippingStatus,
           TrackingId = sd.TrackingId,

              // ✅ Hub matched by store pincode
        HubAddress = _context.HubDetails
            .Where(h => h.Pin == b.PostalCode)
            .Select(h => new HubAddressDto
            {
                HubId = h.HubId,
                HubAddressId = h.HubAddressId,
                HubName = h.HubName,
                AddressLine = h.AddressLine,
                Town = h.Town,
                City = h.City,
                State = h.State,
                Country = h.Country,
                Pin = h.Pin,
                Phone = h.Phone
            })
            .FirstOrDefault()
       }
).ToListAsync();
       

            // ✅ Get DeliveryAddress from ShippingDetails (any one store is enough)
            var deliveryAddress = await _context.ShippingDetails
                .Where(sd => sd.OrderId == orderId)
                .Select(sd => sd.DeliveryAddress)
                .FirstOrDefaultAsync();

            // 3️⃣ Items per store + totals (WITH IMAGE)
            foreach (var store in stores)
            {
                var items = await (
                    from oi in _context.OrderDetails
                    join v in _context.ProductVariantsNew
                        on oi.SkuId equals v.SkuId
                    join p in _context.ProductsNew
                        on v.ProductId equals p.ProductId
                    where oi.StoreOrderId == store.StoreOrderId
                    select new OrderItemDto
                    {
                        ProductName = p.ProductName,
                        Productdesc = p.ProductDescription,
                        Quantity = oi.Quantity,
                        FinalPrice = oi.Price,
                        OriginalPrice = v.Price,
                        DiscountAmount = v.Price - oi.Price,
                        ImageUrl = _context.ProductVariantImagesNew
                            .Where(img => img.SkuId == v.SkuId)
                            .OrderBy(img => img.SortOrder)
                            .Select(img => img.FileName)
                            .FirstOrDefault()
                    }
                ).ToListAsync();

                store.Items = items;
                store.StoreItemsTotal = items.Sum(i => i.FinalPrice * i.Quantity);
            }

            // 4️⃣ Final DTO
            return new OrderConfirmationDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.LatestPayment?.AmountPaid ?? 0,

                ShopperRegId = order.ShopperRegId,
                GuestRegId = order.GuestRegId,
                IsGuestOrder = order.IsGuestOrder,

                ShopperName = order.ShopperName,
                ShopperEmail = order.ShopperEmail,
                ShopperPhone = order.ShopperPhone,

                TransactionId = order.LatestPayment?.PaymentId ?? 0,
                PaymentMethod = order.LatestPayment?.PaymentMethod,

                DeliveryAddress = deliveryAddress,

                Stores = stores
            };
        }

        //reduce stock qty after order is placed
        public async Task ReduceStockAsync(int orderId)
        {
            var skuIds = await (
                from so in _context.StoreOrders
                join oi in _context.OrderDetails on so.StoreOrderId equals oi.StoreOrderId
                where so.OrderId == orderId
                select oi.SkuId
            ).ToListAsync();

            var variants = await _context.ProductVariantsNew
                .Where(v => skuIds.Contains(v.SkuId))
                .ToListAsync();

            foreach (var variant in variants)
            {
                variant.StockQuantity -= 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddShopperNotificationAsync(ShopperDBNotifications notification)
        {
            await _context.ShopperDBNotifications.AddAsync(notification);
        }



        // For multiple transporter orders

      

        private const int MaxP2PHops = 3;

        // Checks the hops the app sent. Returns the plans in leg order, or throws if no longer valid.
        private async Task<List<TransporterTravelPlan>> ValidateChainAsync(
            List<SelectedHopRequestDto> hops, DateTime bookingNow)
        {
            var orderedIds = hops.OrderBy(h => h.LegNumber)
                                 .Select(h => h.TransporterPlanId)
                                 .ToList();

            if (orderedIds.Count == 0 || orderedIds.Count > MaxP2PHops
                || orderedIds.Any(id => id <= 0)
                || orderedIds.Distinct().Count() != orderedIds.Count)
                throw new Exception("Invalid P2P route selected.");

            // every plan must still be active, open and not started
            var plans = await _context.TransporterTravelPlans
                .AsNoTracking()
                .Where(p => orderedIds.Contains(p.PlanId)
                         && p.IsActive
                         && p.PlanStatus == "Available"
                         && p.StartDate > bookingNow)
                .ToListAsync();

            if (plans.Count != orderedIds.Count)
                throw new Exception("Selected P2P transporter plan is no longer available.");

            var chain = orderedIds.Select(id => plans.First(p => p.PlanId == id)).ToList();

            // each next plan starts where the previous ends, after it arrives, by a different transporter
            for (int i = 1; i < chain.Count; i++)
            {
                var prev = chain[i - 1];
                var next = chain[i];

                if (next.StartLocationPin != prev.DestinationPin
                    || next.StartDate < prev.ArrivalDate
                    || next.TransporterRegId == prev.TransporterRegId)
                    throw new Exception("Selected P2P route is no longer valid.");
            }

            return chain;
        }

        // One leg row per plan. IsFinalLeg is the last leg, decided here and never by the app.
        // Setting ShippingDetails = shipping lets EF fill shipping_detail_id when it saves.
        private List<OrderTransportLeg> BuildLegs(
            ShippingDetails shipping, List<TransporterTravelPlan> chain)
        {
            decimal legCost = decimal.Round(shipping.Cost / chain.Count, 2);

            return chain.Select((plan, i) => new OrderTransportLeg
            {
                ShippingDetails = shipping,
                OrderId = shipping.OrderId,
                StoreOrderId = shipping.StoreOrderId,
                LegNumber = i + 1,
                IsFinalLeg = i == chain.Count - 1,
                TransporterPlanId = plan.PlanId,
                TransporterRegId = plan.TransporterRegId,
                FromPin = plan.StartLocationPin,
                ToPin = plan.DestinationPin,
                PlannedStart = plan.StartDate,
                PlannedArrival = plan.ArrivalDate,
                LegStatus = "Assigned",
                HubStatus = "New order",
                LegCost = legCost
            }).ToList();
        }

      

    }
}