using mytown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface IPaymentService
    {
        Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);

        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);

        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

        Task SendCourierEmailAsync(int branchId, int shippingDetailId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);

    }
}
