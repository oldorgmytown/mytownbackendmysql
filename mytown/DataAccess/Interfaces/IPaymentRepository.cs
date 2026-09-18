using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payments> AddPaymentAsync(int orderId, decimal amountPaid, string paymentMethod, string stripePaymentIntentId);

        // check for duplicate payments
        Task<Payments?> GetPaymentByStripePaymentIntentId(string stripePaymentIntentId);
        Task<bool> UpdateCartStatusAsync(int orderId);
        Task<Order> GetOrderWithShippingDetailsAsync(int orderId);

        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);
        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

      //  Task SendEmailToCourier(int branchId, int storeOrderId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);

        // 📦 Products under store order
        List<(string ProductName, int Quantity)> GetProductsByStoreOrderId(int storeOrderId);

        // 🚚 Courier mapping
        Task<int> GetCourierIdByBranchIdAsync(int branchId);

        // 🔔 Notifications
        Task AddCourierNotificationAsync(CourierDBNotifications notification);

        // 💾 Save
        Task SaveChangesAsync();

        // ============================================================
        // P2P Delivery Methods
        // ============================================================

        // Get StoreOrder by ID (needed to parse P2P transporter info)
        Task<StoreOrder?> GetStoreOrderByIdAsync(int storeOrderId);

        // Create P2P delivery request to transporter
        Task CreateP2PDeliveryRequestAsync(CreateP2PDeliveryRequestDto dto);

        // Notify transporter in transporter_delivery_requests table
        Task CreateTransporterNotificationAsync(int transporterRegId, string title, string message);

        // Get store address for pickup location
        Task<string> GetStoreAddressAsync(int storeId);

        // Get total weight of items in store order
        Task<decimal> GetStoreOrderWeightAsync(int storeOrderId);

        // Get item count in store order
        Task<int> GetStoreOrderItemCountAsync(int storeOrderId);

        // Update order status
        Task UpdateOrderStatusAsync(int orderId, string status);

        Task AddTransporterNotificationAsync(TransporterDBNotifications notification);
    }
}