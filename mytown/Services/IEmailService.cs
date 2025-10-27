namespace mytown.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
        Task SendPasswordResetEmail(string email, string resetLink);
        Task SendBusinessnotification(string email, string businessname, int orderId);

        Task SendShopperNotification(string email, string shopperName, int orderId, decimal amountPaid);
        

        Task SendEmailToCourierAsync(string email, string courierName, int shippingDetailId);

        //send email notification to business owner for approval or rejection of profile

        Task SendBusinessStatusEmailAsync(string email, string businessUsername, string businessName, string status);
    }       
}
