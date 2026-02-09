using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using System.Security.Cryptography.X509Certificates;

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
                   // Status = o.ShopperRegister.Status,
                   // Password = o.ShopperRegister.Password,
                    ShopperRegDate = o.ShopperRegister.ShopperRegDate
                })
                .FirstOrDefault(); // Single shopper per order

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

            // ✅ Convert to required tuple format
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
            _context.CourierDBNotifications.Add(notification);
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
                .AsEnumerable() // 👈 important for tuple conversion
                .Select(p => (p.ProductName, p.Quantity))
                .ToList();
        }


    }
}
