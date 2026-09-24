using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
        Task SendPasswordResetEmail(string email, string resetLink);

        // OTP for mobile app
        Task SendOtpEmailAsync(string email, string name, string otp);

        //bussiness conformation email to shopper
        Task SendBusinessnotificationforOrderCnf(string email, string businessname, OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto);

        //shopper conformation email to shopper
        Task SendShopperNotification(string email, string shopperName, OrderConfirmationDto orderdto);

        Task SendBusinessStatusEmailAsync(string email, string businessUsername, string businessName, string status);

        Task SendShopperDeactivationEmailAsync(string email, string shopperName);

        //courier confirmation email
        Task SendEmailToCourierAsync(string email, string courierName, OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto);

        //package rdy email to courier
        Task SendPackagerdyEmailToCourierAsync(string email, string courierName, BusinessOrderDetailsDto dto, string packageSummary);

        //package rdy email to transporter
        Task SendPackagerdyEmailToTransporterAsync(string email, string transporterName, BusinessOrderDetailsDto dto, string packageSummary);

        //transporter confirmation email
        Task SendEmailToTransporterAsync(string email, string transporterName, OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto);

        Task SendShopperReactivationEmailAsync(string email, string shopperName);

        // send email to all branches for login credentials
        Task SendBranchLoginEmailAsync(string email, string password);

        Task SendSenderOrderConfirmationAsync(string email, string shopperName, SenderOrderConfirmationDto dto);

        Task SendTransporterAssignmentAsync(string email, string transporterName, SenderOrderConfirmationDto dto);

        Task SendGuestNotificationforTracking(string email, string guestName, OrderConfirmationDto orderdto);
    }
}