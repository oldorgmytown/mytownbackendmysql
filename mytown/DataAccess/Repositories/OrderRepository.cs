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
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            if (!cartItems.Any())
                return 0;

            decimal totalAmount = cartItems.Sum(c => c.ProductPrice * c.ProdQty);

            var newOrder = new Order
            {
                ShopperRegId = shopperRegId,
                TotalAmount = totalAmount,
                ShippingType = "Multiple", // or leave blank or null
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            List<orderdetails> orderDetailsList = new List<orderdetails>();

            foreach (var item in cartItems)
            {
                var orderDetail = new orderdetails
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.ProductId,
                    StoreId = item.BusRegId,
                    Quantity = item.ProdQty,
                    Price = item.ProductPrice
                };
                orderDetailsList.Add(orderDetail);
            }

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            List<ShippingDetails> shippingList = new List<ShippingDetails>();

            try
            {
                foreach (var orderDetail in orderDetailsList)
                {
                    var shippingSelection = shippingSelections
                        .FirstOrDefault(s => s.StoreId == orderDetail.StoreId);

                    //if (shippingSelection == null)
                    //    throw new Exception($"No shipping selected for store {orderDetail.StoreId}");

                    var shipping = new ShippingDetails
                    {
                        OrderId = newOrder.OrderId,
                        OrderDetailId = orderDetail.OrderDetailId,
                        BranchId = shippingSelection.BranchId,
                        ShippingType = shippingSelection.ShippingType,
                        EstimatedDays = 5, // can be dynamic later
                        Cost = shippingSelection.Cost,
                        TrackingId = "",
                        ShippingStatus = "Ready to Ship"
                    };

                    shippingList.Add(shipping);
                }

                _context.ShippingDetails.AddRange(shippingList);
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("❌ DbUpdateException occurred while saving ShippingDetails:");
                Console.WriteLine(dbEx.InnerException?.Message ?? dbEx.Message);
                throw; // rethrow if needed
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ General exception occurred while saving ShippingDetails:");
                Console.WriteLine(ex.Message);
                throw;
            }

            //foreach (var shippingDetail in shippingList)
            //{
            //    await SendEmailToCourier(shippingDetail.BranchId, shippingDetail.ShippingDetailId);
            //}

            return newOrder.OrderId;
        }

        public async Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId)
        {
            // Step 1: Get all cart items for this shopper
            var cartItems = await (from cart in _context.addtocart
                                   join sku in _context.Sku_ProductVariants
                                   on cart.SkuId equals sku.SkuId
                                   where cart.ShopperRegId == shopperRegId && cart.orderstatus == "Cart"
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
                ShippingType = "Multiple", // Optional
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Step 4: Create order details (use SKU cost instead of product price)
            List<orderdetails> orderDetailsList = cartItems.Select(item => new orderdetails
            {
                OrderId = newOrder.OrderId,
                ProductId = item.cart.ProductId,
                SkuId = item.cart.SkuId,        // Include SKU ID
                StoreId = item.cart.BusRegId,
                Quantity = item.cart.ProdQty ,
                Price = item.sku.Sku_Cost    //  Use SKU cost
            }).ToList();

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            return newOrder.OrderId;
        }

        public async Task SaveShippingSelectionsAsync(int orderId, [FromBody] List<StoreShippingSelection> selections)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            var shippingList = new List<ShippingDetails>();

            foreach (var orderDetail in order.OrderDetails)
            {
                var shippingSelection = selections.FirstOrDefault(s => s.StoreId == orderDetail.StoreId);

                if (shippingSelection == null)
                    throw new Exception($"No shipping selection found for store ID {orderDetail.StoreId}");

                var shipping = new ShippingDetails
                {
                    OrderId = orderId,
                    OrderDetailId = orderDetail.OrderDetailId,
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
