using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using Stripe;

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

        public async Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> shippingSelections)
        {
            // 1️⃣ Get Cart Items
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            if (!cartItems.Any())
                return 0;

            // 2️⃣ Create Order
            decimal totalAmount = cartItems.Sum(c => c.ProductPrice * c.ProdQty);

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

            // 3️⃣ Create StoreOrders (One per Store)
            var groupedStores = cartItems.GroupBy(c => c.BusRegId); // store = BusRegId
            var storeOrders = new List<StoreOrder>();

            foreach (var group in groupedStores)
            {
                var storeOrder = new StoreOrder
                {
                    OrderId = newOrder.OrderId,
                    StoreId = group.Key,
                    StoreTotalAmount = group.Sum(x => x.ProductPrice * x.ProdQty),
                    Storeorder_Status = "Pending"
                };

                storeOrders.Add(storeOrder);
            }

            _context.StoreOrders.AddRange(storeOrders);
            await _context.SaveChangesAsync();

            // Create a lookup for StoreOrderId per StoreId
            var storeOrderMap = storeOrders.ToDictionary(s => s.StoreId, s => s.StoreOrderId);

            // 4️⃣ Create OrderDetails (attach StoreOrderId)
            var orderDetailsList = cartItems.Select(item => new orderdetails
            {
                OrderId = newOrder.OrderId,
                ProductId = item.ProductId,
                StoreId = item.BusRegId,
                Quantity = item.ProdQty,
                Price = item.ProductPrice,
                StoreOrderId = storeOrderMap[item.BusRegId]  // 🔥 Now correctly linked
            }).ToList();

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            // 5️⃣ Create ShippingDetails (one per store)
            var shippingList = new List<ShippingDetails>();

            foreach (var storeOrder in storeOrders)
            {
                var shippingSelection = shippingSelections
                    .FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

                if (shippingSelection == null)
                    continue; // or throw error if mandatory

                var shipping = new ShippingDetails
                {
                    OrderId = newOrder.OrderId,
                    StoreOrderId = storeOrder.StoreOrderId,
                    BranchId = shippingSelection.BranchId,
                    ShippingType = shippingSelection.ShippingType,
                    EstimatedDays = 5,
                    Cost = shippingSelection.Cost,
                    TrackingId = "",
                    ShippingStatus = "Ready to Ship"
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


        public async Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections)
        {
            // Load store orders for this order
            var storeOrders = await _context.StoreOrders
                .Where(s => s.OrderId == orderId)
                .ToListAsync();

            if (!storeOrders.Any())
                throw new Exception("No store orders found for this order.");

            var shippingList = new List<ShippingDetails>();

            foreach (var storeOrder in storeOrders)
            {
                // Find the shipping selection for the store
                var selection = selections.FirstOrDefault(s => s.StoreId == storeOrder.StoreId);

                if (selection == null)
                    throw new Exception($"No shipping selection found for store ID {storeOrder.StoreId}");

                // Create one shipping entry for each store
                var shipping = new ShippingDetails
                {
                    OrderId = orderId,
                    StoreOrderId = storeOrder.StoreOrderId,
                    BranchId = selection.BranchId,
                    ShippingType = selection.ShippingType,
                    EstimatedDays = 5,
                    Cost = selection.Cost,
                    TrackingId = "",                     // empty for now
                    ShippingStatus = "Ready to Ship"
                };

                shippingList.Add(shipping);
            }

            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();

            // Send one email per store order
            foreach (var shippingDetail in shippingList)
            {
                await SendEmailToCourier(shippingDetail.BranchId, shippingDetail.ShippingDetailId);
            }
        }

        private async Task SendEmailToCourier(int branchId, int shippingDetailId)
        {
            var courierInfo = await _context.CourierBranches
                .Where(cb => cb.BranchId == branchId)
                .Select(cb => new
                {
                    cb.CourierName,
                    cb.CourierId,
                    CourierEmail = cb.CourierService.CourierEmail
                })
                .FirstOrDefaultAsync();

            if (courierInfo != null && !string.IsNullOrEmpty(courierInfo.CourierEmail))
            {
                await _emailService.SendEmailToCourierAsync(
                    courierInfo.CourierEmail,
                    courierInfo.CourierName,
                    shippingDetailId
                );
            }
        }


    }
}
