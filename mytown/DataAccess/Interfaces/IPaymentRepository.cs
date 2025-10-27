using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);
        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);
        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

        Task SendEmailToCourier(int branchId, int shippingDetailId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);


    }
}
