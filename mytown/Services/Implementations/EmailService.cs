using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using mytown.Services.Interfaces;
using mytown.Models.DTO_s;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;
    private readonly string _senderEmail;

    public EmailService(IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        _smtpServer = emailSettings["SmtpServer"];
        _smtpPort = 587; // Use from config if needed
        _smtpUser = emailSettings["SenderEmail"];
        _smtpPass = emailSettings["SenderPassword"];
        _senderEmail = _smtpUser;
    }

    public async Task SendVerificationEmail(string email, string verificationLink)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");


        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true; // Ensure SSL/TLS is enabled

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Email Verification - MyTown",
                    Body = $@"
<div style='font-family: Arial, sans-serif; background-color: #ffffff; padding: 40px; text-align: center;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 30px; border-radius: 10px; 
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2); border: 2px solid #004481;'>
        
        <!-- Itismytown Logo -->
        <img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' 
             alt='Google Logo' width='120' style='margin-bottom: 20px;' />

        <!-- Email Header -->
        <h2 style='color: #004481; margin-bottom: 10px;'>Verify your email address</h2>

        <p style='color: #333; font-size: 14px;'>
            Please confirm that you want to use this as your MyTown account email address. 
            Once it's done, you will be able to access your account.
        </p>

        <!-- Verification Button -->
        <a href='{verificationLink}' 
           style='display: inline-block; background-color: #004481; color: white; padding: 12px 24px; 
                  text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold; margin: 20px 0;'>
            Verify email
        </a>

        <!-- Alternative Text Link -->
        <p style='color: #333; font-size: 12px;'>Or paste this link into your browser:</p>
        <p style='word-break: break-word; font-size: 12px;'>
            <a href='{verificationLink}' style='color: #004481;'>{verificationLink}</a>
        </p>

        <hr style='border: 0.5px solid #ddd; margin: 20px 0;' />

        <!-- Footer -->
        <p style='font-size: 10px; color: #777;'>© 2025 MyTown. All rights reserved.</p>

 
  
    </div>
</div>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);
                                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw new Exception("Failed to send verification email.");
        }
    }

    //<!-- Additional Success Button -->
    //       <hr style='margin: 30px 0;' />
    //       <a href='{{successUrl}}' 
    //          style='display: inline-block; background-color: #28a745; color: white; padding: 12px 24px;
    //                 text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold;'>
    //           Go to My Account
    //       </a>

    //       <p style='font-size: 10px; color: #777; margin-top: 20px;'>If you've already verified, click the button above to continue.</p>

    public async Task SendPasswordResetEmail(string email, string resetLink)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Password Reset - MyTown",
                    Body = $@"
<div style='font-family: Arial, sans-serif; background-color: #ffffff; padding: 40px; text-align: center;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 30px; border-radius: 10px; 
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2); border: 2px solid #004481;'>
        
        <!-- Itismytown Logo -->
        <img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' 
             alt='Google Logo' width='120' style='margin-bottom: 20px;' />

        <!-- Email Header -->
        <h2 style='color: #004481; margin-bottom: 10px;'>Verify your email address</h2>

        <p style='color: #333; font-size: 14px;'>
            Please confirm that you want to use this as your MyTown account email address. 
            Once it's done, you will be able to access your account.
        </p>

        <!-- Verification Button -->
        <a href='{resetLink}' 
           style='display: inline-block; background-color: #004481; color: white; padding: 12px 24px; 
                  text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold; margin: 20px 0;'>
            Reset Password
        </a>

        <!-- Alternative Text Link -->
        <p style='color: #333; font-size: 12px;'>Or paste this link into your browser:</p>
        <p style='word-break: break-word; font-size: 12px;'>
            <a href='{resetLink}' style='color: #004481;'>{resetLink}</a>
        </p>

        <hr style='border: 0.5px solid #ddd; margin: 20px 0;' />

        <!-- Footer -->
        <p style='font-size: 10px; color: #777;'>© 2025 MyTown. All rights reserved.</p>

 
    </div>
</div>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending password reset email: {ex.Message}");
            throw new Exception("Failed to send password reset email.");
        }
    }

    public async Task SendBusinessnotificationforOrderCnf(string email, string businessname, OrderConfirmationDto orderdto, StoreOrderConfirmationDto storedto)

    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Purchase Notiifcation",
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    <h3 style='color: #000;'>Notification to business owner – Email content</h3>
    <p>Dear <strong>{businessname}</strong>,</p>

    <p>
      We are writing to confirm that payment has been successfully completed for an order placed through our online platform.
      Kindly proceed with processing and shipping the product for the order ID <strong></strong>.
      Check for the order details in your business portal.
    </p>

    <p>
      Please update the shipping status on the platform once the order has been dispatched.
      If you encounter any issues or need further assistance, feel free to contact us.
    </p>

    <p>Thank you for your prompt attention to this order.</p>

    <p style='margin-top: 30px;'>
      Best regards,<br />
      <strong style='color: #004481;'>ItIsMyTown</strong><br />
      <em>[Contact Details]</em>
    </p>
  </body>
</html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending business notifcation email: {ex.Message}");
            throw new Exception("Failed to send business notifcation email.");
        }
    }

    public async Task SendShopperNotification(string email, string shopperName, OrderConfirmationDto orderdto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Order Payment Confirmation",
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    <h3 style='color: #000;'>Payment Confirmation – Thank You for Your Purchase!</h3>

    <p>Dear <strong>{shopperName}</strong>,</p>

    <p>
      We are delighted to inform you that your payment for 
      <strong>Order ID #</strong> has been successfully processed.
    </p>

    <p>
      <strong>Payment Details:</strong><br />
      Amount Paid: <strong>₹</strong><br />
      Payment Date: <strong></strong>
    </p>

    <p>
      Our partner stores have been notified and will begin preparing your order for shipment.
      You can track your order status anytime by logging in to your account.
    </p>

    <p>
      If you have any questions or need assistance, please don’t hesitate to reach out to our support team.
    </p>

    <p style='margin-top: 30px;'>
      Thank you for shopping with <strong style='color: #004481;'>ItIsMyTown</strong>!<br />
      We hope to serve you again soon.
    </p>

    <hr style='border: none; border-top: 1px solid #ccc; margin: 25px 0;' />
    <p style='font-size: 0.9em; color: #666;'>
      <em>This is an automated message. Please do not reply directly to this email.</em><br/>
      <strong>ItIsMyTown</strong><br/>
      support@itismytown.com | +91-XXXXXXXXXX
    </p>
  </body>
</html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending shopper notification email: {ex.Message}");
            throw new Exception("Failed to send shopper notification email.");
        }
    }

    private async Task<bool> DomainHasMX(string email)
    {
        try
        {
            var domain = email.Split('@')[1];
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain, QueryType.MX);
            return result.Answers.MxRecords().Any();
        }
        catch
        {
            return false;
        }
    }

    //send notification to courier
    public async Task SendEmailToCourierAsync(string email, string courierName, int shippingDetailId)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "New Shipment Notification",
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    <h3 style='color: #000;'>Shipping Notification – Assigned to Your Branch</h3>
    <p>Hello <strong>{courierName}</strong>,</p>

    <p>
      A new shipment task has been assigned to your branch. Please prepare for dispatch using the information below.
    </p>

    <p>
      <strong>Shipping ID:</strong> {shippingDetailId}<br />
      <strong>Assigned Date:</strong> {DateTime.Now:dd-MMM-yyyy hh:mm tt}
    </p>

    <p>
      Kindly login to your courier portal and update the status as soon as the parcel is dispatched.
      Ensure timely pickup and delivery to meet our service standards.
    </p>

    <p>If you have any questions or face any issues, please contact the operations team.</p>

    <p style='margin-top: 30px;'>
      Best regards,<br />
      <strong style='color: #004481;'>ItIsMyTown Logistics</strong><br />
      <em>[Contact Details]</em>
    </p>
  </body>
</html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending courier email: {ex.Message}");
            throw new Exception("Failed to send courier shipping notification email.");
        }
    }

    //Admin Approval or Rejection of submitted business profile

    public async Task SendBusinessStatusEmailAsync(string email, string businessUsername, string businessName, string status)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                string statusLower = status.ToLower();
                string statusColor = statusLower == "approved" ? "#28a745" : "#dc3545"; // green or red
                string statusText = status.ToUpper();

                // Different message for Approved or Rejected
                string messageContent = statusLower switch
                {
                    "approved" => $@"
                    <p>
                      Congratulations! Your business profile <strong>({businessName})</strong> has been 
                      <span style='color: {statusColor}; font-weight: bold;'>APPROVED</span> by our admin team.
                    </p>
                    <p>
                      You can now log in and start using all features of the platform. Welcome aboard!
                    </p>",

                    "rejected" => $@"
                    <p>
                      We regret to inform you that your business profile <strong>({businessName})</strong> 
                      has been <span style='color: {statusColor}; font-weight: bold;'>REJECTED</span> after review.
                    </p>
                    <p>
                      Please check the details you submitted and ensure they meet our registration criteria. 
                      You may revise your profile and resubmit for approval.
                    </p>",

                    _ => $@"
                    <p>
                      The status of your business profile <strong>({businessName})</strong> has been updated to 
                      <span style='color: {statusColor}; font-weight: bold;'>{statusText}</span>.
                    </p>"
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Your Business Profile Status Update",
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    <h3 style='color: #000;'>Business Profile Status Notification</h3>

    <p>Hello <strong>{businessUsername}</strong>,</p>

    {messageContent}

    <p>
      <strong>Updated On:</strong> {DateTime.Now:dd-MMM-yyyy hh:mm tt}
    </p>

    <p style='margin-top: 30px;'>
      Best regards,<br />
      <strong style='color: #004481;'>ItIsMyTown Business Support</strong><br />
      <em>[Contact Details]</em>
    </p>
  </body>
</html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending business status email: {ex.Message}");
            throw new Exception("Failed to send business status notification email.");
        }
    }

    public async Task SendShopperDeactivationEmailAsync(string email, string shopperName)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Your Account Has Been Deactivated",
                    IsBodyHtml = true,
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>

    <h3 style='color:#000;'>Account Deactivation Notice</h3>

    <p>Dear <strong>{shopperName}</strong>,</p>

    <p>
      We would like to inform you that your shopper account has been
      <strong>deactivated</strong> by the administrator.
    </p>

    <p>
      As a result, you will no longer be able to place orders or access your account.
    </p>

    <p>
      If you believe this action was taken in error or if you require further clarification,
      please contact our support team.
    </p>

    <p style='margin-top: 30px;'>
      Best regards,<br />
      <strong style='color:#004481;'>MyTown Support Team</strong><br />
      <em>Customer Care</em>
    </p>

  </body>
</html>"
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending shopper deactivation email: {ex.Message}");
            throw new Exception("Failed to send shopper deactivation email.");
        }
    }



}
