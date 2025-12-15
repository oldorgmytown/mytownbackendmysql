using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
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

        public Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod)
        {
            // Check if the order exists
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            // Create new payment entry
            var payment = new Payments
            {
                OrderId = orderId,
                AmountPaid = amountPaid,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Completed", 
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Update order status to "Paid"
            order.OrderStatus = "Paid";
            _context.Orders.Update(order);
            _context.SaveChanges();

           

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
                           .ToList();
        }

        public async Task SendEmailToCourier(int branchId, int shippingDetailId)
        {
            var courierInfo = await _context.CourierBranches
                .Where(cb => cb.BranchId == branchId)
                .Select(cb => new
                {
                    cb.CourierServiceName,
                    cb.CourierId,
                    CourierEmail = cb.CourierService.CourierEmail
                })
                .FirstOrDefaultAsync();
            //retrive main office email with branch id
            if (courierInfo != null && !string.IsNullOrEmpty(courierInfo.CourierEmail))
            {
                await _emailService.SendEmailToCourierAsync(
                    courierInfo.CourierEmail,
                    courierInfo.CourierServiceName,
                    shippingDetailId
                );
            }
        }

    }
}
