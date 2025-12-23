using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
        Task SendPasswordResetEmail(string email, string resetLink);
        Task SendBusinessnotificationforOrderCnf(string email, string businessname,OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto);

        Task SendShopperNotification(string email, string shopperName, OrderConfirmationDto orderdto);
        

        Task SendEmailToCourierAsync(string email, string courierName, int shippingDetailId);

        //send email notification to business owner for approval or rejection of profile

        Task SendBusinessStatusEmailAsync(string email, string businessUsername, string businessName, string status);
    }       
}
