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

        //public async Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> shippingSelections)
        //{
        //    // 1️⃣ Get Cart Items
        //    var cartItems = await _context.addtocart
        //        .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
        //        .ToListAsync();

        //    if (!cartItems.Any())
        //        return 0;

        //    // 2️⃣ Create Order
        //    decimal totalAmount = cartItems.Sum(c => c.ProductPrice * c.ProdQty);

        //    var newOrder = new Order
        //    {
        //        ShopperRegId = shopperRegId,
        //        TotalAmount = totalAmount,
        //        ShippingType = "Multiple",
        //        OrderStatus = "Pending",
        //        OrderDate = DateTime.UtcNow
        //    };

        //    _context.Orders.Add(newOrder);
        //    await _context.SaveChangesAsync();

        //    // 3️⃣ Create StoreOrders (One per Store)
        //    var groupedStores = cartItems.GroupBy(c => c.BusRegId); // store = BusRegId
        //    var storeOrders = new List<StoreOrder>();

        //    foreach (var group in groupedStores)
        //    {
        //        var storeOrder = new StoreOrder
        //        {
        //            OrderId = newOrder.OrderId,
        //            StoreId = group.Key,
        //            StoreTotalAmount = group.Sum(x => x.ProductPrice * x.ProdQty),
        //            Storeorder_Status = "Pending"
        //        };

        //        storeOrders.Add(storeOrder);
        //    }

        //    _context.StoreOrders.AddRange(storeOrders);
        //    await _context.SaveChangesAsync();

        //    // Create a lookup for StoreOrderId per StoreId
        //    var storeOrderMap = storeOrders.ToDictionary(s => s.StoreId, s => s.StoreOrderId);

        //    // 4️⃣ Create OrderDetails (attach StoreOrderId)
        //    var orderDetailsList = cartItems.Select(item => new orderdetails
        //    {
        //        OrderId = newOrder.OrderId,
        //        ProductId = item.ProductId,
        //        StoreId = item.BusRegId,
        //        Quantity = item.ProdQty,
        //        Price = item.ProductPrice,
        //        StoreOrderId = storeOrderMap[item.BusRegId]  
        //    }).ToList();

        //    _context.OrderDetails.AddRange(orderDetailsList);
        //    await _context.SaveChangesAsync();

        //    // 5️⃣ Create ShippingDetails (one per store)
        //    // 5️⃣ Create ShippingDetails (one per store)
        //    var shippingList = new List<ShippingDetails>();

        //    foreach (var storeOrder in storeOrders)
        //    {
        //        var shippingSelection = shippingSelections
        //            .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

        //        if (shippingSelection == null)
        //            continue; // or throw exception if required

        //        // 🔹 Fetch courier branch
        //        var branch = await _context.CourierBranches
        //            .FirstOrDefaultAsync(b => b.BranchId == shippingSelection.BranchId);

        //        if (branch == null)
        //            throw new Exception($"Courier branch not found for BranchId {shippingSelection.BranchId}");

        //        var shipping = new ShippingDetails
        //        {
        //            OrderId = newOrder.OrderId,
        //            StoreOrderId = storeOrder.StoreOrderId,
        //            BranchId = branch.BranchId,
        //            ShippingType = shippingSelection.ShippingType,

        //            // ✅ Get estimated days from CourierBranch
        //            EstimatedDays = branch.EstimateDays ?? 0,

        //            Cost = branch.Charges,
        //            TrackingId = "",
        //            ShippingStatus = "Ready to Ship"
        //        };

        //        shippingList.Add(shipping);
        //    }

        //    _context.ShippingDetails.AddRange(shippingList);
        //    await _context.SaveChangesAsync();


        //    return newOrder.OrderId;
        //}

        public async Task<int> CreateOrderAsync(
            int shopperRegId,
            int? selectedAltAddressId,
            List<StoreShippingSelection> shippingSelections)
        {
            // 1️⃣ Get Cart Items
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            if (!cartItems.Any())
                return 0;

            // 2️⃣ Create Order
                     

            decimal totalAmount = cartItems.Sum(c => c.ProductPrice * c.ProdQty);

            // Normalize alternate address id
            if (selectedAltAddressId.HasValue && selectedAltAddressId.Value <= 0)
            {
                selectedAltAddressId = null;
            }

            var newOrder = new Order
            {
                ShopperRegId = shopperRegId,
                SelectedAltAddressId = selectedAltAddressId,
                TotalAmount = totalAmount,
                ShippingType = "Multiple",
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // 🔹 Resolve delivery address ONCE (THIS IS THE KEY PART)
            string deliveryAddress;
           

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
                    .Where(s => s.ShopperRegId == shopperRegId)
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

            // 3️⃣ Create StoreOrders
            var groupedStores = cartItems.GroupBy(c => c.BusRegId);
            var storeOrders = new List<StoreOrder>();

            // Shipping per store - New
            var shippingMap = shippingSelections.ToDictionary(s => s.StoreId);

            foreach (var group in groupedStores)
            {
                if (!shippingMap.TryGetValue(group.Key, out var shipping))
                    throw new Exception($"Shipping selection missing for StoreId {group.Key}");
                storeOrders.Add(new StoreOrder
                {
                    OrderId = newOrder.OrderId,
                    StoreId = group.Key,
                    StoreTotalAmount = group.Sum(x => x.ProductPrice * x.ProdQty),
                    Storeorder_Status = "Pending",
                    CourierType = shipping.ShippingType // new added
                });
            }

            _context.StoreOrders.AddRange(storeOrders);
            await _context.SaveChangesAsync();

            var storeOrderMap = storeOrders.ToDictionary(
                s => s.StoreId,
                s => s.StoreOrderId
            );


            // 3️⃣🔔 Create Notifications for each Store
            var notifications = storeOrders.Select(so => new BusinessDBNotifications
            {
                BusRegId = so.StoreId,
                Title = "New Order Received",
                Message = $"Order #{newOrder.OrderId} has been placed",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            _context.BusinessDBNotifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // 4️⃣ Create OrderDetails
            var orderDetailsList = cartItems.Select(item => new orderdetails
            {
                OrderId = newOrder.OrderId,
                ProductId = item.ProductId,
                SkuId = item.SkuId,
                StoreId = item.BusRegId,
                Quantity = item.ProdQty,
                Price = item.ProductPrice,
                StoreOrderId = storeOrderMap[item.BusRegId]
            }).ToList();

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            // 5️⃣ Create ShippingDetails (WITH DELIVERY ADDRESS STORED)
            var shippingList = new List<ShippingDetails>();

            foreach (var storeOrder in storeOrders)
            {
                var shippingSelection = shippingSelections
                    .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

                if (shippingSelection == null)
                    continue;

                var branch = await _context.CourierBranches
                    .FirstOrDefaultAsync(b => b.BranchId == shippingSelection.BranchId);

                if (branch == null)
                    throw new Exception($"Courier branch not found for BranchId {shippingSelection.BranchId}");

                // ✅ FETCH SERVICE (THIS IS THE FIX)

                //convert Standard -- >Surface && Express --> Air
                string dbShippingMode = shippingSelection.ShippingType.ToLower() switch
                {
                    "standard" => "Surface",
                    "express" => "Air",
                    _ => throw new Exception("Invalid shipping type")
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
                    ShippingType = shippingSelection.ShippingType,
                    EstimatedDays = service.EstimateDays ?? 0,
                    Cost = service.Charges,
                    TrackingId = "",
                    ShippingStatus = "In Progress",

                    //  THIS IS WHAT YOU ASKED FOR
                    DeliveryAddress = deliveryAddress
                };

                shippingList.Add(shipping);
            }

            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();

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


        //public async Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections)
        //{
        //    // Load store orders for this order
        //    var storeOrders = await _context.StoreOrders
        //        .Where(s => s.OrderId == orderId)
        //        .ToListAsync();

        //    if (!storeOrders.Any())
        //        throw new Exception("No store orders found for this order.");

        //    var shippingList = new List<ShippingDetails>();

        //    foreach (var storeOrder in storeOrders)
        //    {
        //        // Find the shipping selection for the store
        //        var selection = selections.FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

        //        if (selection == null)
        //            throw new Exception($"No shipping selection found for store ID {storeOrder.StoreId}");

        //        // Create one shipping entry for each store
        //        var shipping = new ShippingDetails
        //        {
        //            OrderId = orderId,
        //            StoreOrderId = storeOrder.StoreOrderId,
        //            BranchId = selection.BranchId,
        //            ShippingType = selection.ShippingType,
        //            EstimatedDays = 5,
        //            Cost = selection.Cost,
        //            TrackingId = "",                     // empty for now
        //            ShippingStatus = "Ready to Ship"
        //        };

        //        shippingList.Add(shipping);
        //    }

        //    _context.ShippingDetails.AddRange(shippingList);
        //    await _context.SaveChangesAsync();

        //    // Send one email per store order
        //    foreach (var shippingDetail in shippingList)
        //    {
        //        await SendEmailToCourier(shippingDetail.BranchId, shippingDetail.ShippingDetailId);
        //    }
        //}

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

                // ✅ FETCH SERVICE (THIS IS THE FIX)
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

                    ShippingType = selection.ShippingType,   // or branch.ShippingMode
                    EstimatedDays = service.EstimateDays.Value,
                   // EstimatedDeliveryDate = orderDate.AddDays(branch.EstimateDays.Value),

                    Cost = service.Charges,

                    TrackingId = null,
                    ShippingStatus = "In Progress"
                };

                shippingList.Add(shipping);
            }

            // 6️⃣ Save all shipping records
            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();

            //// 7️⃣ Notify courier (one email per store)
            //foreach (var shippingDetail in shippingList)
            //{
            //    await SendEmailToCourier(
            //        shippingDetail.BranchId,
            //        shippingDetail.ShippingDetailId);
            //}
        }


        //private async Task SendEmailToCourier(int branchId, int shippingDetailId)
        //{
        //    var courierInfo = await _context.CourierBranches
        //        .Where(cb => cb.BranchId == branchId)
        //        .Select(cb => new
        //        {
        //            cb.CourierServiceName,
        //            cb.CourierId,
        //            CourierEmail = cb.CourierService.CourierEmail
        //        })
        //        .FirstOrDefaultAsync();

        //    if (courierInfo != null && !string.IsNullOrEmpty(courierInfo.CourierEmail))
        //    {
        //        await _emailService.SendEmailToCourierAsync(
        //            courierInfo.CourierEmail,
        //            courierInfo.CourierServiceName,
        //            shippingDetailId
        //        );
        //    }
        //}

        public async Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId)
        {
            // 1️⃣ Order + Shopper basic details + PaymentMethod
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.ShopperRegId,

                    ShopperName = o.ShopperRegister.Username,
                    ShopperEmail = o.ShopperRegister.Email,
                    ShopperPhone = o.ShopperRegister.PhoneNumber,

                    // ✅ PaymentMethod from Payments table
                    PaymentMethod = _context.Payments
                        .Where(p => p.OrderId == o.OrderId)
                        .OrderByDescending(p => p.PaymentDate)
                        .Select(p => p.PaymentMethod)
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
                where so.OrderId == orderId
                select new StoreOrderConfirmationDto
                {
                    StoreOrderId = so.StoreOrderId,
                    StoreId = so.StoreId,
                    StoreName = b.BusinessName,
                    BusinessEmail = b.BusEmail,

                    ShippingType = sd.ShippingType,
                    ShippingAmount = sd.Cost,
                    EstimatedDays = sd.EstimatedDays,
                    EstimatedDeliveryDate = order.OrderDate.AddDays(sd.EstimatedDays),
                    ShippingStatus = sd.ShippingStatus
                }
            ).ToListAsync();

            // ✅ Get DeliveryAddress from ShippingDetails (any one store is enough)
            var deliveryAddress = await _context.ShippingDetails
                .Where(sd => sd.OrderId == orderId)
                .Select(sd => sd.DeliveryAddress)
                .FirstOrDefaultAsync();

            // 3️⃣ Items per store + totals
            foreach (var store in stores)
            {
                var items = await (
                    from oi in _context.OrderDetails
                    join p in _context.products
                        on oi.ProductId equals p.ProductId
                    where oi.StoreOrderId == store.StoreOrderId
                    select new OrderItemDto
                    {
                        ProductName = p.ProductName,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }
                ).ToListAsync();

                store.Items = items;
                store.StoreItemsTotal = items.Sum(i => i.Price * i.Quantity);
            }

            // 4️⃣ Final DTO
            return new OrderConfirmationDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,

                ShopperRegId = order.ShopperRegId,
                ShopperName = order.ShopperName,
                ShopperEmail = order.ShopperEmail,
                ShopperPhone = order.ShopperPhone,

                PaymentMethod = order.PaymentMethod,
                DeliveryAddress = deliveryAddress,

                Stores = stores
            };
        }



    }
}
