using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod, string stripePaymentIntentId);

        Task<Order> GetOrderWithShippingDetailsAsync(int orderId);
        //  Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);
        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);
        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

        Task SendEmailToCourier(int branchId, int storeOrderId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);

        // 📦 Products under store order
        List<(string ProductName, int Quantity)> GetProductsByStoreOrderId(int storeOrderId);

        // 🚚 Courier mapping
        Task<int> GetCourierIdByBranchIdAsync(int branchId);

        // 🔔 Notifications
        Task AddCourierNotificationAsync(CourierDBNotifications notification);

        // 💾 Save
        Task SaveChangesAsync();


    }
}
