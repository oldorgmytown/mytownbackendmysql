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
using System.Text;

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
        var htmlBody = BuildShopperNotificationTemplate(
            WebUtility.HtmlEncode(shopperName),
            orderdto);

        using (var smtpClient = new SmtpClient(_smtpServer))
        {
            smtpClient.Port = _smtpPort;
            smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
            smtpClient.EnableSsl = true;

            using (var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail, "ITISMYTOWN"),
                Subject = $"Order Confirmation - {orderdto.OrderId}",
                Body = htmlBody,
                IsBodyHtml = true
            })
            {
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        Console.WriteLine($"Order confirmation email sent to {email}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending shopper notification email: {ex.Message}");
        throw new Exception("Failed to send shopper notification email.");
    }
}

private string BuildShopperNotificationTemplate(string shopperName, OrderConfirmationDto orderdto)
{
    var storesBuilder = new StringBuilder();

    foreach (var store in orderdto.Stores)
    {
        var itemsBuilder = new StringBuilder();

        foreach (var item in store.Items)
        {
            itemsBuilder.Append($@"
<tr>
    <td style='padding:8px 0;font-size:14px;color:#585858;'>
        {WebUtility.HtmlEncode(item.ProductName)}
    </td>
    <td style='padding:8px 0;font-size:14px;color:#585858;text-align:center;'>
        Qty: {item.Quantity}
    </td>
    <td style='padding:8px 0;font-size:14px;color:#000;text-align:right;'>
        ₹{item.ItemTotal:F2}
    </td>
</tr>");
        }

        storesBuilder.Append($@"
<table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:20px;border:1px solid #E5E7EB;border-radius:6px;'>
<tr>
    <td style='padding:16px;'>
        <table width='100%' cellpadding='0' cellspacing='0'>
            <tr>
                <td style='font-size:18px;font-weight:600;color:#000;'>
                    {WebUtility.HtmlEncode(store.StoreName)}
                </td>
                <td style='font-size:16px;font-weight:700;color:#000;text-align:right;'>
                    ₹{store.StoreTotal:F2}
                </td>
            </tr>
        </table>

        <table width='100%' cellpadding='0' cellspacing='0' style='margin-top:10px;'>
            {itemsBuilder}
        </table>

        <table width='100%' cellpadding='0' cellspacing='0' style='margin-top:12px;background:#F0FFF4;border:1px solid #BBF7D0;border-radius:4px;'>
            <tr>
                <td style='padding:10px;font-size:14px;color:#166534;'>
                    <strong>{WebUtility.HtmlEncode(store.ShippingStatus)}</strong><br/>
                    Expected delivery by {store.EstimatedDeliveryDate:MMMM dd, yyyy}
                </td>
            </tr>
        </table>
    </td>
</tr>
</table>");
    }

    return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Order Confirmation</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0;padding:0;background:#FAFBFC;font-family:Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='background:#FAFBFC;padding:20px 0;'>
<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 4px rgba(0,0,0,0.1);'>

<!-- Header with Logo -->
<tr>
<td style='padding:20px;text-align:center;border-bottom:1px solid #E5E7EB;background:#ffffff;'>
<img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' height='50' alt='ITISMYTOWN Logo'/>
</td>
</tr>

<!-- Hero Section -->
<tr>
<td style='padding:30px;text-align:center;background:#004481;color:#ffffff;'>
<h1 style='margin:0;font-size:24px;font-weight:bold;'>Order Confirmed!</h1>
<p style='margin:8px 0 0;font-size:14px;'>Thank you for shopping with us</p>
</td>
</tr>

<!-- Main Content -->
<tr>
<td style='padding:30px;'>

<p style='font-size:16px;color:#000;margin:0 0 10px 0;'>
Hello <strong>{shopperName}</strong>,
</p>

<p style='font-size:14px;color:#585858;margin:0 0 20px 0;'>
Your order has been successfully placed and is being processed.
</p>

<!-- Order Summary -->
<table width='100%' cellpadding='0' cellspacing='0' style='margin:20px 0;border:1px solid #E5E7EB;border-radius:6px;background:#F9FAFB;'>
<tr>
<td style='padding:15px;font-size:14px;color:#333;'>
<strong style='color:#000;'>Order ID:</strong> {WebUtility.HtmlEncode(orderdto.OrderId.ToString())}<br/>
<strong style='color:#000;'>Order Date:</strong> {orderdto.OrderDate:MMMM dd, yyyy}<br/>
<strong style='color:#000;'>Total Amount:</strong> <span style='color:#004481;font-weight:bold;'>₹{orderdto.TotalAmount:F2}</span>
</td>
</tr>
</table>

<!-- Store Sections -->
<h3 style='font-size:16px;color:#000;margin:20px 0 15px 0;font-weight:bold;'>Order Details</h3>
{storesBuilder}

<!-- Payment & Delivery Info -->
<table width='100%' cellpadding='0' cellspacing='0' style='margin-top:20px;'>
<tr>
<td style='font-size:14px;color:#000;margin-bottom:15px;'>
<strong>Payment Method:</strong> {WebUtility.HtmlEncode(orderdto.PaymentMethod)}
</td>
</tr>
<tr>
<td style='font-size:14px;color:#000;padding-top:10px;border-top:1px solid #E5E7EB;'>
<strong>Delivery Address:</strong><br/>
<span style='color:#585858;'>{WebUtility.HtmlEncode(orderdto.ShopperName)}<br/>
{WebUtility.HtmlEncode(orderdto.DeliveryAddress)}<br/>
{WebUtility.HtmlEncode(orderdto.ShopperPhone)}</span>
</td>
</tr>
</table>

<!-- CTA Button -->
<div style='text-align:center;margin:30px 0;'>
<a href='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/orders/{WebUtility.HtmlEncode(orderdto.OrderId.ToString())}'
style='background:#004481;color:#ffffff;padding:12px 24px;text-decoration:none;border-radius:6px;font-size:14px;font-weight:bold;display:inline-block;'>
View Order Details
</a>
</div>

<!-- Support Message -->
<p style='font-size:12px;color:#585858;margin:20px 0 0 0;text-align:center;'>
If you have any questions, please contact our support team.
</p>

</td>
</tr>

<!-- Footer -->
<tr>
<td style='padding:20px;background:#F3F4F6;text-align:center;font-size:11px;color:#585858;border-top:1px solid #E5E7EB;'>
<p style='margin:0;'>© 2026 ITISMYTOWN. All rights reserved.</p>
<p style='margin:5px 0 0 0;'>This is an automated message. Please do not reply directly to this email.</p>
</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";
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

    // 📧 Send notification email to courier (ONE email per StoreOrder)
    public async Task SendEmailToCourierAsync(
        string email,
        string courierName,
        int storeOrderId,
        string storeName,
        List<(string ProductName, int Quantity)> products)
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

                // 🧾 Build product list HTML
                var productHtml = string.Join("", products.Select(p =>
                    $"<li>{p.ProductName} × {p.Quantity}</li>"
                ));

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "New Store Shipment Assigned",
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    
    <h3 style='color: #000;'>📦 New Store Shipment Assigned</h3>

    <p>Hello <strong>{courierName}</strong>,</p>

    <p>
      A new shipment has been assigned to your branch. Below are the shipment details:
    </p>

    <p>
      <strong>Store Order ID:</strong> {storeOrderId}<br />
      <strong>Store Name:</strong> {storeName}<br />
      <strong>Assigned Date:</strong> {DateTime.Now:dd-MMM-yyyy hh:mm tt}
    </p>

    <h4>Products to Ship:</h4>
    <ul>
      {productHtml}
    </ul>

    <p>
      Please arrange pickup and update the shipment status in the courier portal.
    </p>

    <p style='margin-top: 30px;'>
      Best regards,<br />
      <strong style='color: #004481;'>ItIsMyTown Logistics</strong>
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
            throw new Exception("Failed to send courier shipment email.");
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


    public async Task SendShopperReactivationEmailAsync(string email, string shopperName)
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
                    Subject = "Your Account Has Been Reactivated",
                    IsBodyHtml = true,
                    Body = $@"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>

    <h3 style='color:#000;'>Account Reactivation Notice</h3>

    <p>Dear <strong>{shopperName}</strong>,</p>

    <p>
      Good news! Your shopper account has been
      <strong>reactivated</strong> by the administrator.
    </p>

    <p>
      You can now log in and continue placing orders and accessing your account as usual.
    </p>

    <p>
      If you have any questions or need assistance, please feel free to contact our support team.
    </p>

    <p style='margin-top: 30px;'>
      Welcome back!<br /><br />
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
            Console.WriteLine($"Error sending shopper reactivation email: {ex.Message}");
            throw new Exception("Failed to send shopper reactivation email.");
        }
    }


}
