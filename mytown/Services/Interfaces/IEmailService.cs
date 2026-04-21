using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
        Task SendPasswordResetEmail(string email, string resetLink);
        Task SendBusinessnotificationforOrderCnf(string email, string businessname,OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto);

        Task SendShopperNotification(string email, string shopperName, OrderConfirmationDto orderdto);
        

       // Task SendEmailToCourierAsync(string email, string courierName, int shippingDetailId);

        //send email notification to business owner for approval or rejection of profile

        Task SendBusinessStatusEmailAsync(string email, string businessUsername, string businessName, string status);

        Task SendShopperDeactivationEmailAsync(string email, string shopperName);

        Task SendEmailToCourierAsync(
       string email,
       string courierName,
       OrderConfirmationDto orderdto,
       StoreOrderConfirmationDto storedto);

        //package rdy email to courier

        Task SendPackagerdyEmailToCourierAsync(
       string email,
       string courierName,
       BusinessOrderDetailsDto dto,
       string packageSummary);

        Task SendPackagerdyEmailToTransporterAsync(
       string email,
       string transporterName,
       BusinessOrderDetailsDto dto,string packageSummary);

        Task SendEmailToTransporterAsync(
    string email,
    string transporterName,
    OrderConfirmationDto orderdto,
    StoreOrderConfirmationDto storedto);
        Task SendShopperReactivationEmailAsync(string email, string shopperName);

        // send email to all branches for login credentials
        Task SendBranchLoginEmailAsync(string email, string password);



    }
}
