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
using Microsoft.AspNetCore.Session;

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
                smtpClient.UseDefaultCredentials = false;
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
        <img src='https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png' 
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
            Console.WriteLine(ex.ToString());
            throw;
        }
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Error sending email: {ex.Message}");
        //    throw new Exception("Failed to send verification email.");
        //}
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

                smtpClient.UseDefaultCredentials = false;   // Important for Microsoft SMTP

                smtpClient.Credentials = new NetworkCredential(
                    _smtpUser,
                    _smtpPass
                );

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
        <img src='https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png' 
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

   


    //order email confirmation to shopper


    public async Task SendBusinessnotificationforOrderCnf(
  string email,
  string businessname,
  OrderConfirmationDto orderdto,
  StoreOrderConfirmationDto storedto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;

                smtpClient.UseDefaultCredentials = false;   // Important for Microsoft SMTP

                smtpClient.Credentials = new NetworkCredential(
                    _smtpUser,
                    _smtpPass
                );

                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = $"New Order Received - {storedto.StoreOrderId}",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                // =============================
                // BUILD DYNAMIC ITEMS HTML
                // =============================
                var itemsHtml = new StringBuilder();
                var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";
                string shippingPartnerLabel =
    storedto.ShippingType.Equals("P2P", StringComparison.OrdinalIgnoreCase)
    ? "Transporter"
    : "Courier";

                foreach (var item in storedto.Items)
                {
                    itemsHtml.Append($@"
<tr>
  <td style=""padding:0 0 12px 0;border-bottom:1px solid #E5E7EB;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
      <tr>
        <td width=""64"" style=""padding-right:12px;"" valign=""middle"">
          <img src=""{imageBaseUrl}/{Uri.EscapeDataString(item.ImageUrl)}""
               width=""48"" height=""48""
               style=""width:48px;height:48px;border-radius:8px;object-fit:cover;display:block;"" />
        </td>
        <td valign=""middle""
            style=""color:#585858;font-size:14px;font-weight:500;line-height:1.4;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          {item.ProductName}
        </td>
        <td width=""60"" align=""right"" valign=""middle""
            style=""color:#585858;font-size:14px;font-weight:500;white-space:nowrap;padding-left:8px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Qty: {item.Quantity}
        </td>
        <td width=""80"" align=""right"" valign=""middle""
            style=""color:#585858;font-size:14px;font-weight:600;white-space:nowrap;padding-left:8px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          &#8377;{item.ItemTotal:N2}
        </td>
      </tr>
    </table>
  </td>
</tr>");
                }

                // =============================
                // FULL EMAIL BODY
                // =============================
                var body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>New Order Received - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/order_recive.png""
            alt=""Order Confirmed""
            width=""600""
            style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + ORDER INFO BLOCK ===== -->
    <tr>
      <td style=""padding:20px 30px;border-bottom:1px solid #fff;"">

        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {storedto.StoreName},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0 0 16px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          You have received a new order from {orderdto.ShopperName}. Please prepare the items for shipment.
        </p>

        <!-- Order ID / Store Order ID / Date / Total -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:4px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td style=""padding-right:12px;"">
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
              </div>
            </td>
            <td style=""padding-right:12px;"">
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Order ID</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/biz/orders/{storedto.StoreOrderId}""
                   style=""color:#004481;text-decoration:underline;font-weight:600;
                          font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                  {storedto.StoreOrderId}
                </a>
              </div>
            </td>
            <td style=""padding-right:12px;"">
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
            <td>
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Total Amount</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                &#8377;{storedto.StoreItemsTotal:N2}
              </div>
            </td>
          </tr>
        </table>

        <!-- ===== CUSTOMER INFORMATION ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Customer Information</div>
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
                <tr>
                  <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:14px;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Customer Name</td>
                  <td align=""right"" style=""color:#585858;font-size:14px;font-weight:600;padding-bottom:14px;
                                             font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    {orderdto.ShopperName}
                  </td>
                </tr>
                <tr>
                  <td colspan=""2"" style=""padding-bottom:14px;"">
                    <div style=""height:1px;background:#E5E7EB;""></div>
                  </td>
                </tr>
                <tr>
                  <td style=""color:#585858;font-size:14px;font-weight:500;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Phone Number</td>
                  <td align=""right"" style=""color:#585858;font-size:14px;font-weight:600;
                                             font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    {orderdto.ShopperPhone}
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>

        <!-- ===== ITEMS ORDERED ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Items Ordered</div>
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid #E5E7EB;border-radius:12px;padding:12px;"">
                {itemsHtml}
              </table>
            </td>
          </tr>
        </table>

        <!-- ===== PAYMENT METHOD ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:12px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Payment Method</div>
              <p style=""color:#585858;font-size:14px;font-weight:500;margin:0 0 4px 0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.PaymentMethod}
              </p>
              <p style=""color:#22A048;font-size:14px;font-weight:600;margin:0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Payment successful
              </p>
            </td>
          </tr>
        </table>

        <!-- ===== ESTIMATED DELIVERY DATE ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:12px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Estimated Delivery Date</div>
              <p style=""color:#585858;font-size:14px;font-weight:500;margin:0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {storedto.EstimatedDeliveryDate:MMMM dd, yyyy}
              </p>
            </td>
          </tr>
        </table>

<!-- ===== SHIPPING METHOD ===== -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
              border-radius:8px;padding:24px;margin-bottom:16px;"">
  <tr>
    <td>

      <!-- Shipping Method -->
      <div style=""color:#585858;font-size:13px;font-weight:500;margin-bottom:4px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        Shipping Method
      </div>
      <div style=""color:#000;font-size:15px;font-weight:600;margin-bottom:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        {storedto.ShippingType} ({shippingPartnerLabel})
      </div>

      <!-- Divider -->
      <div style=""height:1px;background:#F1F1F3;margin:16px 0;""></div>

     <!-- Dynamic Partner Info -->
<div style=""color:#585858;font-size:13px;font-weight:500;margin-bottom:4px;"">
  {shippingPartnerLabel} Name
</div>
<div style=""color:#000;font-size:15px;font-weight:600;margin-bottom:12px;"">
  {(shippingPartnerLabel == "Transporter" ? storedto.TransporterName : storedto.CourierName)}
</div>

<div style=""color:#585858;font-size:13px;font-weight:500;margin-bottom:4px;"">
  {shippingPartnerLabel} Phone
</div>
<div style=""color:#000;font-size:15px;font-weight:600;"">
  {(shippingPartnerLabel == "Transporter" ? storedto.TransporterPhone : storedto.CourierPhone)}
</div>

    </td>
  </tr>
</table>

        <!-- ===== DELIVERY ADDRESS ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:12px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Delivery Address</div>
              <p style=""color:#585858;font-size:14px;font-weight:600;margin:0 0 4px 0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.ShopperName}
              </p>
              <p style=""color:#585858;font-size:14px;font-weight:500;line-height:1.5;margin:0 0 8px 0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.DeliveryAddress}
              </p>
              <table cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td width=""22"" height=""22"" align=""center"" valign=""middle""
                      style=""background:#F5F5F5;border-radius:50%;width:28px;height:28px;padding-right:8px;"">
                    &#128222;
                  </td>
                  <td style=""color:#585858;font-size:14px;font-weight:500;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    {orderdto.ShopperPhone}
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>

        <!-- ===== ACTION REQUIRED BANNER ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #F7BBBB;border-radius:8px;
                      background:#FDF0F0;padding:12px 16px;margin-bottom:16px;"">
          <tr>
            <td>
              <p style=""color:#FF1434;font-size:14px;line-height:1.5;margin:0;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                <strong style=""display:block;margin-bottom:4px;"">Action Required</strong>
                Please process this order and update the shipping status.
                The customer is expecting delivery by {storedto.EstimatedDeliveryDate:MMMM dd, yyyy}.
              </p>
            </td>
          </tr>
        </table>

        <!-- ===== VIEW ORDER DETAILS BUTTON ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding:8px 0 0 0;"">
              <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/biz/orders/{storedto.StoreOrderId}""
                 style=""display:inline-block;background:#004481;color:#fff;
                        border:1px solid #004481;border-radius:8px;
                        padding:14px 40px;font-size:16px;font-weight:400;
                        text-decoration:none;text-align:center;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                View Order Details
              </a>
            </td>
          </tr>
        </table>

      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Seller Dashboard</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Management</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;line-height:1.5;padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you&#39;re a seller on our platform.
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;line-height:1.5;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";

                mailMessage.Body = body;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending business notification email: {ex.Message}");
            throw new Exception("Failed to send business notification email.");
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
                smtpClient.UseDefaultCredentials = false;
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
            var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";

            foreach (var item in store.Items)
            {
                string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                    ? "https://via.placeholder.com/64x64?text=No+Image"
                    : item.ImageUrl;

                itemsBuilder.Append($@"
<tr>
  <td style=""padding: 0 0 12px 0; border-bottom: 1px solid #E5E7EB;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
      <tr>
        <td width=""64"" style=""padding-right: 12px;"" valign=""middle"">
          <img src=""{imageBaseUrl}/{Uri.EscapeDataString(imageSrc)}"" alt=""Product""
               width=""48"" height=""48""
               style=""width:48px;height:48px;border-radius:6px;object-fit:cover;display:block;"" />
        </td>
        <td valign=""middle"" style=""color:#585858;font-size:14px;font-weight:600;line-height:20px;"">
          {WebUtility.HtmlEncode(item.ProductName)}
        </td>
        <td width=""60"" align=""right"" valign=""middle""
            style=""color:#585858;font-size:14px;font-weight:500;white-space:nowrap;padding-left:8px;"">
          Qty: {item.Quantity}
        </td>
        <td width=""80"" align=""right"" valign=""middle""
            style=""color:#585858;font-size:14px;font-weight:600;white-space:nowrap;padding-left:8px;"">
          &#8377;{item.ItemTotal:F2}
        </td>
      </tr>
    </table>
  </td>
</tr>");
            }

            storesBuilder.Append($@"
<!-- ===== STORE BLOCK ===== -->
<tr>
  <td style=""padding: 20px 24px; border-bottom: 1px solid #E5E7EB; background:#fff;"">

    <!-- Store header row -->
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-bottom:12px;"">
      <tr>
        <td valign=""top"">
          <div style=""color:#000;font-size:20px;font-weight:600;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;margin-bottom:4px;"">
            {WebUtility.HtmlEncode(store.StoreName)}
          </div>
        </td>
        <td align=""right"" valign=""top"">
          <div style=""color:#000;font-size:16px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Total</div>
          <div style=""color:#585858;font-size:16px;font-weight:700;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8377;{store.StoreTotal:F2}</div>
        </td>
      </tr>
    </table>

    <!-- Store Order ID + EDD -->
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-bottom:12px;"">
      <tr>
        <td style=""color:#000;font-size:16px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;padding-bottom:6px;"">Store Order ID</td>
        <td align=""right"" style=""padding-bottom:6px;"">
  <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/shopper/order-details/{store.StoreOrderId}""
     style=""color:#004481;font-size:16px;font-weight:500;text-decoration:underline;
            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
    {store.StoreOrderId}
  </a>
</td>
      </tr>
      <tr>
        <td style=""color:#000;font-size:16px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Estimated Date of Delivery</td>
        <td align=""right"" style=""color:#585858;font-size:16px;font-weight:500;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">{store.EstimatedDeliveryDate:MMMM dd, yyyy}</td>
      </tr>
      <tr>
        <td style=""color:#000;font-size:16px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Shipping Type</td>
        <td align=""right"" style=""color:#585858;font-size:16px;font-weight:500;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">{store.ShippingType}</td>
      </tr>
    </table>

    <!-- Items heading -->
    <div style=""color:#000;font-size:18px;font-weight:500;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;margin-bottom:12px;margin-top:12px;"">Items in your order</div>

    <!-- Items list -->
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
           style=""border:1px solid #E5E7EB;border-radius:12px;padding:12px;"">
      {itemsBuilder}
    </table>

    <!-- Shipping status -->
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:12px;"">
      <tr>
        <td style=""background:#F0FFF4;border:1px solid #BBF7D0;border-radius:8px;padding:12px;"">
          <span style=""color:#166534;font-size:14px;font-weight:600;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            {WebUtility.HtmlEncode(store.ShippingStatus)}
          </span><br/>
          <span style=""color:#166534;font-size:14px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            Expected delivery by {store.EstimatedDeliveryDate:MMMM dd, yyyy}
          </span>
        </td>
      </tr>
    </table>

  </td>
</tr>");
        }

        decimal subtotal = orderdto.Stores.Sum(s => s.StoreItemsTotal);
        decimal shipping = orderdto.Stores.Sum(s => s.ShippingAmount);
        decimal tax = subtotal * 0.18m;
        decimal grandTotal = subtotal + shipping + tax;

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Order Confirmation - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"" style=""padding:0;"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/shopper_email.png""
            alt=""Order Confirmed""
            width=""600""
            style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + ORDER INFO ===== -->
    <tr>
      <td style=""padding:20px 30px;border-bottom:1px solid #fff;"">

        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {WebUtility.HtmlEncode(shopperName)},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0 0 16px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Your order has been confirmed and will be shipping soon.
        </p>

        <!-- Order ID / Date / Total row -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.08);border-radius:4px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td style=""padding-right:16px;"">
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:4px;
             font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>
<div style=""color:#585858;font-size:16px;font-weight:600;
             font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
  ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
</div>
            </td>
            <td style=""padding-right:16px;"">
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
            <td>
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Total Amount</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                &#8377;{orderdto.TotalAmount:F2}
              </div>
            </td>
          </tr>
        </table>

        <!-- ===== STORES TABLE ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid rgba(139,139,139,0.08);border-radius:4px;overflow:hidden;"">
          {storesBuilder}
        </table>

      </td>
    </tr>

    <!-- ===== ORDER SUMMARY ===== -->
    <tr>
      <td style=""padding:24px 30px;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Summary</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</td>
            <td align=""right"" style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Subtotal</td>
            <td align=""right"" style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{subtotal:F2}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Shipping</td>
            <td align=""right"" style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{shipping:F2}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Tax</td>
            <td align=""right"" style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{tax:F2}
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:12px;"">
              <div style=""height:1px;background:#D9D9D9;""></div>
            </td>
          </tr>
          <tr>
            <td style=""color:#000;font-size:16px;font-weight:600;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Total Amount</td>
            <td align=""right"" style=""color:#000;font-size:16px;font-weight:600;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{grandTotal:F2}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== PAYMENT METHOD ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 12px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Payment Method</h2>
        <p style=""color:#585858;font-size:14px;font-weight:500;margin:0 0 4px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          {WebUtility.HtmlEncode(orderdto.PaymentMethod)}
        </p>
        <p style=""color:#16A34A;font-size:14px;font-weight:500;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Payment successful
        </p>
      </td>
    </tr>



    <!-- ===== DELIVERY ADDRESS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 12px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Delivery Address</h2>
        <p style=""color:#000;font-size:14px;font-weight:600;margin:0 0 4px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          {WebUtility.HtmlEncode(orderdto.ShopperName)}
        </p>
        <p style=""color:#585858;font-size:14px;font-weight:500;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          {WebUtility.HtmlEncode(orderdto.DeliveryAddress)}
        </p>
        <table cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td width=""22"" height=""22"" align=""center"" valign=""middle""
                style=""background:#F5F5F5;border-radius:50%;width:28px;height:28px;padding-right:8px;"">
              &#128222;
            </td>
            <td style=""color:#585858;font-size:14px;font-weight:500;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(orderdto.ShopperPhone)}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== VIEW ORDER BUTTON ===== -->
    <tr>
      <td align=""center"" style=""padding:24px 30px;"">
        <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/shopper/orders""
           style=""display:inline-block;background:#004481;color:#fff;border:1px solid #004481;
                  border-radius:8px;padding:14px 40px;font-size:16px;font-weight:400;
                  text-decoration:none;text-align:center;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          View Order
        </a>
      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Continue Shopping</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">My Account</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                                        padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you placed an order with us.
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                                        padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
          <tr>
            <td align=""center"">
              <a href=""#"" style=""color:#004481;font-size:12px;font-weight:400;
                                   text-decoration:underline;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Unsubscribe from marketing emails
              </a>
            </td>
          </tr>
        </table>
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
    public async Task SendEmailToCourierAsync(
      string email,
      string courierName,
      OrderConfirmationDto orderdto,
      StoreOrderConfirmationDto storedto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                // ✅ Use new template (same structure as transporter)
                string body = BuildCourierNotificationTemplate(
                    courierName,
                    orderdto,
                    storedto
                );

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Delivery Assignment Confirmed",
                    Body = body,
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
    private string BuildCourierNotificationTemplate(
        string courierName,
        OrderConfirmationDto orderdto,
        StoreOrderConfirmationDto storedto)
    {
        var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";

        // ===== BUILD PRODUCT ROWS =====
        var productsBuilder = new StringBuilder();

        foreach (var item in storedto.Items)
        {
            string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                ? "https://via.placeholder.com/80x80?text=No+Image"
                : item.ImageUrl;

            productsBuilder.Append($@"
<!-- Product Card -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""border:1px solid #E5E7EB;border-radius:12px;margin-bottom:12px;"">
  <tr>
    <td style=""padding:12px;"">

      <!-- Top: image + details -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
          <td width=""80"" valign=""top"" style=""padding-right:12px;"">
            <img src=""{imageBaseUrl}/{Uri.EscapeDataString(imageSrc)}""
                 alt=""Product"" width=""80"" height=""80""
                 style=""width:80px;height:80px;border-radius:8px;object-fit:cover;display:block;background:#F5F5F5;"" />
          </td>
          <td valign=""top"">
            <div style=""color:#52525B;font-size:14px;font-weight:600;line-height:20px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.ProductName)}
            </div>
            <div style=""color:#9CA3AF;font-size:12px;font-weight:400;line-height:16px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.Productdesc ?? string.Empty)}
            </div>
            
          </td>
        </tr>
      </table>

      <!-- Bottom: qty + price -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:12px;"">
        <tr>
          <td style=""color:#52525B;font-size:14px;font-weight:500;
                      font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            Qty: {item.Quantity}
          </td>
          <td align=""right"" style=""color:#52525B;font-size:16px;font-weight:600;
                                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            &#8377;{item.ItemTotal:F2}
          </td>
        </tr>
      </table>

    </td>
  </tr>
</table>");
        }

        // ===== FULL EMAIL BODY =====
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Delivery Assignment Confirmed - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
         style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER ===== -->
    <tr>
      <td align=""center""
          style=""padding:20px 28px 24px;border-bottom:1px solid #F4F4F5;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/deliver_comform.jpeg""
            alt=""Order Confirmed""
            width=""600""
            style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + ORDER INFO ===== -->
    <tr>
      <td style=""padding:20px 28px 24px;border-bottom:1px solid #fff;"">

        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {WebUtility.HtmlEncode(courierName)},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0 0 20px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          You have been assigned a new delivery from
          <strong>{WebUtility.HtmlEncode(storedto.StoreName)}</strong>.
          Please collect the package and deliver it to the customer address.
        </p>

        <!-- Order ID + Order Date -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
                      border-radius:4px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td style=""padding-right:16px;"">
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
             font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>

              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
              </div>
            </td>
            <td>
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
          </tr>
        </table>

        <!-- ===== STORE INFORMATION ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Store Information
              </div>

              <!-- Store card -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid rgba(0,0,0,0.10);border-radius:8px;padding:16px;"">
                <tr>
                  <td>
                    <!-- Store name + address -->
                    <div style=""color:#000;font-size:20px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(storedto.StoreName)}
                    </div>
                    <div style=""color:#9CA3AF;font-size:12px;font-weight:500;line-height:16px;margin-bottom:16px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(storedto.StoreAddress ?? string.Empty)}
                    </div>

                    <!-- Store Order ID / Phone / Email rows -->
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:12px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Store Order ID
                        </td>
                        <td align=""right"" style=""padding-bottom:6px;"">
                              <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/courier/orders/{storedto.StoreOrderId}""
                                 style=""color:#004481;font-size:16px;font-weight:500;text-decoration:underline;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                                {storedto.StoreOrderId}
                              </a>
                            </td>
                      
                        
                      </tr>
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:12px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Phone Number
                        </td>
                        <td align=""right"" style=""color:#52525B;font-size:14px;font-weight:600;padding-bottom:12px;
                                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          {WebUtility.HtmlEncode(storedto.BusinessPhone ?? string.Empty)}
                        </td>
                      </tr>
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Email
                        </td>
                        <td align=""right"" style=""color:#52525B;font-size:14px;font-weight:600;
                                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          {WebUtility.HtmlEncode(storedto.BusinessEmail ?? string.Empty)}
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== PRODUCTS TO DELIVER ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid rgba(113,113,122,0.10);border-radius:8px;margin-bottom:16px;"">
          <tr>
            <td style=""background:#fff;border-radius:8px;padding:20px 24px;"">

              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Products to Deliver
              </div>

              {productsBuilder}

              <!-- Expected Delivery Date banner -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td style=""background:#F0FFF4;border:1px solid #BBF7D0;
                               border-radius:8px;padding:12px 16px;"">
                    <div style=""color:#14532D;font-size:12px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Expected Delivery Date
                    </div>
                    <div style=""color:#14532D;font-size:12px;font-weight:400;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Please deliver by {storedto.EstimatedDeliveryDate:MMMM dd, yyyy}
                    </div>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== CUSTOMER INFORMATION ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Customer Information
              </div>

              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid rgba(0,0,0,0.10);border-radius:12px;padding:12px;"">
                <tr>
                  <td style=""padding-bottom:12px;"">
                    <!-- Name -->
                    <div style=""color:#000;font-size:16px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperName)}
                    </div>
                    <!-- Phone -->
                    <div style=""color:#6B7280;font-size:14px;font-weight:500;margin-bottom:2px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperPhone)}
                    </div>
                    <!-- Email -->
                    <div style=""color:#6B7280;font-size:14px;font-weight:500;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperEmail ?? string.Empty)}
                    </div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <!-- Address -->
                    <div style=""color:#52525B;font-size:14px;font-weight:400;line-height:1.5;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.DeliveryAddress)}
                    </div>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== VIEW ORDER DETAILS BUTTON ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding:8px 0 0 0;"">

              <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/courier/orders/{storedto.StoreOrderId}""
                 style=""display:inline-block;background:#0C4A6E;color:#fff;
                        border:1px solid #0C4A6E;border-radius:8px;
                        padding:14px 40px;font-size:16px;font-weight:400;
                        text-decoration:none;text-align:center;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                View Order Details
              </a>
            </td>
          </tr>
        </table>

      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(113,113,122,0.10);padding:20px 28px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Seller Dashboard</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Management</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center""
                style=""color:#52525B;font-size:12px;font-weight:400;line-height:1.5;
                        padding-bottom:6px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you&#39;re a seller on our platform.
            </td>
          </tr>
          <tr>
            <td align=""center""
                style=""color:#52525B;font-size:12px;font-weight:400;line-height:1.5;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";
    }



    // ================================
    // READY TO SHIP EMAIL TO COURIER
    // ================================
    public async Task SendPackagerdyEmailToCourierAsync(
        string email,
        string courierName,
        BusinessOrderDetailsDto dto,
        string packageSummary)
    {
        if (!await DomainHasMX(email))
            throw new Exception("Invalid email domain.");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials =
                    new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                string body =
                    BuildReadyToShipCourierTemplate(courierName, dto,packageSummary);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Package Ready for Pickup",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Failed to send courier email.");
        }
    }

    private string BuildReadyToShipCourierTemplate(
    string courierName,
    BusinessOrderDetailsDto dto,
    string packageSummary)
{
    return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Package Ready for Pickup - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"" style=""padding:0;"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER (LOGO) ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <!-- ===== BANNER IMAGE ===== -->
    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/ready_to_deliver.jpeg""
             alt=""Package Ready for Pickup""
             width=""600""
             style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + INTRO ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {WebUtility.HtmlEncode(courierName)},
        </p>
        <p style=""color:#585858;font-size:16px;font-weight:400;line-height:1.5;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          A shipment is ready for pickup. Please collect from the store and deliver to the customer.
        </p>
      </td>
    </tr>

    <!-- ===== ORDER INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Order ID</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.StoreOrderId}
            </td>
          </tr>

          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.OrderDate:dd MMM yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Expected Delivery</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.EstimatedDeliveryDate:dd MMM yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Package Details</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(packageSummary)}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== STORE PICKUP DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Name</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.StoreName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.StoreTown)}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== DELIVERY DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Delivery Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Customer</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.CustomerName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Phone</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.CustomerPhone)}
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:12px;"">
              <div style=""height:1px;background:#D9D9D9;""></div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"">
              <div style=""color:#000;font-size:14px;font-weight:600;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Shipping Address
              </div>
              <div style=""color:#585858;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ShippingAddress)}
              </div>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== VIEW ORDER BUTTON ===== -->
    <tr>
      <td align=""center"" style=""padding:24px 30px;"">
        <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/courier/orders/{dto.StoreOrderId}""
           style=""display:inline-block;background:#004481;color:#fff;border:1px solid #004481;
                  border-radius:8px;padding:14px 40px;font-size:16px;font-weight:400;
                  text-decoration:none;text-align:center;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          View Order
        </a>
      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                                        padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";
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
                smtpClient.UseDefaultCredentials = false;
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
                smtpClient.UseDefaultCredentials = false;
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
                smtpClient.UseDefaultCredentials = false;
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

    public async Task SendBranchLoginEmailAsync(string email, string password)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Courier Branch Login Credentials - MyTown",
                    Body = $@"
<div style='font-family: Arial, sans-serif; background-color: #ffffff; padding: 40px; text-align: center;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 30px; border-radius: 10px; 
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2); border: 2px solid #004481;'>

        <!-- MyTown Logo -->
        <img src='https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png' 
             alt='MyTown Logo' width='120' style='margin-bottom: 20px;' />

        <h2 style='color: #004481; margin-bottom: 10px;'>Courier Branch Login Created</h2>

        <p style='color: #333; font-size: 14px;'>
            Your courier branch account has been successfully created in the MyTown system.
        </p>

        <div style='background-color:#f5f7fa;padding:15px;border-radius:6px;margin:20px 0;'>
            <p style='font-size:14px;margin:5px 0;'><b>Login Email:</b> {email}</p>
            <p style='font-size:14px;margin:5px 0;'><b>Default Password:</b> {password}</p>
        </div>

        <p style='color:#333;font-size:13px;'>
            For security reasons, we recommend changing your password after your first login.
        </p>

        <hr style='border: 0.5px solid #ddd; margin: 20px 0;' />

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
            throw new Exception("Failed to send branch login email.");
        }
    }

    public async Task SendEmailToTransporterAsync(
    string email,
    string transporterName,
    OrderConfirmationDto orderdto,
    StoreOrderConfirmationDto storedto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;


                string body = BuildTransporterNotificationTemplate(
                    transporterName,
                    orderdto,
                    storedto
                );

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Transport Order Assigned",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending transporter email: {ex.Message}");
            throw new Exception("Failed to send transporter email.");
        }
    }

    private string BuildTransporterNotificationTemplate(
      string transporterName,
      OrderConfirmationDto orderdto,
      StoreOrderConfirmationDto storedto)
    {
        var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";

        // ===== BUILD PRODUCT ROWS =====
        var productsBuilder = new StringBuilder();

        foreach (var item in storedto.Items)
        {
            string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                ? "https://via.placeholder.com/80x80?text=No+Image"
                : item.ImageUrl;

            productsBuilder.Append($@"
<!-- Product Card -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""border:1px solid #E5E7EB;border-radius:12px;margin-bottom:12px;"">
  <tr>
    <td style=""padding:12px;"">

      <!-- Top: image + details -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
          <td width=""80"" valign=""top"" style=""padding-right:12px;"">
            <img src=""{imageBaseUrl}/{Uri.EscapeDataString(imageSrc)}""
                 alt=""Product"" width=""80"" height=""80""
                 style=""width:80px;height:80px;border-radius:8px;object-fit:cover;display:block;"" />
          </td>
          <td valign=""top"">
            <div style=""color:#585858;font-size:14px;font-weight:600;line-height:20px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.ProductName)}
            </div>
            <div style=""color:#9CA3AF;font-size:12px;font-weight:400;line-height:16px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.Productdesc?? string.Empty)}
            </div>
           
          </td>
        </tr>
      </table>

      <!-- Bottom: qty + price -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:8px;"">
        <tr>
          <td style=""color:#585858;font-size:14px;font-weight:500;
                      font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            Qty: {item.Quantity}
          </td>
          <td align=""right"" style=""color:#585858;font-size:16px;font-weight:600;
                                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            &#8377;{item.ItemTotal:F2}
          </td>
        </tr>
      </table>

    </td>
  </tr>
</table>");
        }

        // ===== FULL EMAIL BODY =====
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Transport Order Confirmed - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
         style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER ===== -->
    <tr>
      <td align=""center""
          style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/order_conform.jpeg""
            alt=""Order Confirmed""
            width=""600""
            style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + ORDER INFO ===== -->
    <tr>
      <td style=""padding:20px 30px;"">

        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {WebUtility.HtmlEncode(transporterName)},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0 0 20px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          You have been assigned a new delivery from
          <strong>{WebUtility.HtmlEncode(storedto.StoreName)}</strong>.
          Please collect the package and deliver it to the customer address.
        </p>

        <!-- Order ID + Order Date -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.10);
                      border-radius:4px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td style=""padding-right:16px;"">
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
              </div>
            </td>
            <td>
              <div style=""color:#585858B;font-size:14px;font-weight:500;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
          </tr>
        </table>

        <!-- ===== STORE INFORMATION ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.10);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Store Information
              </div>

              <!-- Store Name card -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid rgba(0,0,0,0.10);border-radius:8px;padding:16px;"">
                <tr>
                  <td>
                    <!-- Store name + address -->
                    <div style=""color:#000;font-size:20px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(storedto.StoreName)}
                    </div>
                    <div style=""color:#9CA3AF;font-size:12px;font-weight:500;line-height:16px;margin-bottom:2px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(storedto.StoreAddress ?? string.Empty)}
                    </div>

                    <!-- Store Order ID / Phone / Email rows -->
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:12px;"">
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:8px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Store Order ID
                        </td>
                                <td align=""right"" style=""padding-bottom:6px;"">
                                  <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/transporter/my-plans""
                                     style=""color:#004481;font-size:16px;font-weight:500;text-decoration:underline;
                                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                                    {storedto.StoreOrderId}
                                  </a>
                                </td>

                      </tr>
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:8px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Phone Number
                        </td>
                        <td align=""right"" style=""color:#585858;font-size:14px;font-weight:600;padding-bottom:8px;
                                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          {WebUtility.HtmlEncode(storedto.BusinessPhone?? string.Empty)}
                        </td>
                      </tr>
                      <tr>
                        <td style=""color:#6B7280;font-size:14px;font-weight:500;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Email
                        </td>
                        <td align=""right"" style=""color:#585858;font-size:14px;font-weight:600;
                                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          {WebUtility.HtmlEncode(storedto.BusinessEmail ?? string.Empty)}
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== PRODUCTS TO DELIVER ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.10);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Products to Deliver
              </div>

              {productsBuilder}

              <!-- Expected Delivery Date banner -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:4px;"">
                <tr>
                  <td style=""background:#F0FFF4;border:1px solid #BBF7D0;
                               border-radius:8px;padding:12px 16px;"">
                    <div style=""color:#166534;font-size:12px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Expected Delivery Date
                    </div>
                    <div style=""color:#166534;font-size:12px;font-weight:400;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Please deliver by {storedto.EstimatedDeliveryDate:MMMM dd, yyyy}
                    </div>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== CUSTOMER INFORMATION ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(139,139,139,0.10);
                      border-radius:8px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td>
              <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Customer Information
              </div>

              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid rgba(0,0,0,0.10);border-radius:12px;padding:12px;"">
                <tr>
                  <td>
                    <!-- Name / Phone / Email -->
                    <div style=""color:#000;font-size:16px;font-weight:600;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperName)}
                    </div>
                    <div style=""color:#6B7280;font-size:14px;font-weight:500;margin-bottom:2px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperPhone)}
                    </div>
                    <div style=""color:#6B7280;font-size:14px;font-weight:500;margin-bottom:12px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.ShopperEmail ?? string.Empty)}
                    </div>

                    <!-- Delivery address -->
                    <div style=""color:#585858;font-size:14px;font-weight:400;line-height:1.5;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      {WebUtility.HtmlEncode(orderdto.DeliveryAddress)}
                    </div>
                  </td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

        <!-- ===== VIEW ORDER DETAILS BUTTON ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding:8px 0 16px 0;"">
              <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/transporter/my-plans""
                 style=""display:inline-block;background:#004481;color:#fff;
                        border:1px solid #004481;border-radius:8px;
                        padding:14px 40px;font-size:16px;font-weight:400;
                        text-decoration:none;text-align:center;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                View Order Details
              </a>
            </td>
          </tr>
        </table>

      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Seller Dashboard</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Management</a>
              <a href=""#"" style=""color:#004481;font-size:16px;font-weight:400;
                                   text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center""
                style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                        padding-bottom:6px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you&#39;re a transporter on our platform.
            </td>
          </tr>
          <tr>
            <td align=""center""
                style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";
    }



    // ========================================
    // READY TO SHIP EMAIL TO TRANSPORTER
    // ========================================
    public async Task SendPackagerdyEmailToTransporterAsync(
        string email,
        string transporterName,
        BusinessOrderDetailsDto dto,
        string packageSummary)
    {
        if (!await DomainHasMX(email))
            throw new Exception("Invalid email domain.");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials =
                    new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                string body =
                    BuildReadyToShipTransporterTemplate(
                        transporterName,
                        dto, packageSummary
                    );

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Package Ready for Pickup",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(
                "Failed to send transporter email."
            );
        }
    }

    private string BuildReadyToShipTransporterTemplate(
    string transporterName,
    BusinessOrderDetailsDto dto,
    string packageSummary)
{
    return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Package Ready for Pickup - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"" style=""padding:0;"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER (LOGO) ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <!-- ===== BANNER IMAGE ===== -->
    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/ready_to_deliver.jpeg""
             alt=""Package Ready for Pickup""
             width=""600""
             style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>

    <!-- ===== HELLO + INTRO ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {WebUtility.HtmlEncode(transporterName)},
        </p>
        <p style=""color:#585858;font-size:16px;font-weight:400;line-height:1.5;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          A shipment is ready for pickup. Please collect from the store and deliver to the customer.
        </p>
      </td>
    </tr>

    <!-- ===== ORDER INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Order ID</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.StoreOrderId}
            </td>
          </tr>
          
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.OrderDate:dd MMM yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Expected Delivery</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.EstimatedDeliveryDate:dd MMM yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Package Details</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(packageSummary)}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== FROM (STORE PICKUP DETAILS) ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">From</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Name</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.StoreName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.StoreTown)}
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== ABOUT (DELIVERY DETAILS) ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">About</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""border:1px solid #E5E7EB;border-radius:12px;padding:20px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Customer</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.CustomerName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:500;padding-bottom:12px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Phone</td>
            <td align=""right"" style=""color:#000;font-size:14px;font-weight:600;padding-bottom:12px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.CustomerPhone)}
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:12px;"">
              <div style=""height:1px;background:#D9D9D9;""></div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"">
              <div style=""color:#000;font-size:14px;font-weight:600;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Shipping Address
              </div>
              <div style=""color:#585858;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ShippingAddress)}
              </div>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== VIEW ORDER BUTTON ===== -->
    <tr>
      <td align=""center"" style=""padding:24px 30px;"">
        <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/transporter/my-plans""
           style=""display:inline-block;background:#004481;color:#fff;border:1px solid #004481;
                  border-radius:8px;padding:14px 40px;font-size:16px;font-weight:400;
                  text-decoration:none;text-align:center;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          View Order
        </a>
      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;
                                        padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";
}

    // ============================================================
    // METHOD 1: Sender Order Confirmation Email
    // ============================================================
    public async Task SendSenderOrderConfirmationAsync(
        string email,
        string senderName,
        SenderOrderConfirmationDto dto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                string body = BuildSenderOrderConfirmationTemplate(
                    WebUtility.HtmlEncode(senderName),
                    dto);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "ITISMYTOWN"),
                    Subject = $"Shipment Booking Confirmed - {dto.SenderOrderId}",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }

            Console.WriteLine($"Shipment booking confirmation email sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending shipment booking confirmation email: {ex.Message}");
            throw new Exception("Failed to send shipment booking confirmation email.");
        }
    }

    private string BuildSenderOrderConfirmationTemplate(
        string senderName,
        SenderOrderConfirmationDto dto)
    {
        decimal gst = dto.TransportationCharge * 0.18m;
        decimal totalPaid = dto.TransportationCharge + gst;

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Shipment Booking Confirmed - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
 
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"" style=""padding:0;"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">
 
    <!-- ===== HEADER (LOGO) ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>
 
    <!-- ===== BANNER IMAGE ===== -->
    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/ready_to_deliver.jpeg""
             alt=""Booking Order Confirmed""
             width=""600""
             style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>
 
    <!-- ===== HELLO + INTRO ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {senderName},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Your shipment booking has been confirmed! Transporter is ready for pickup.
        </p>
      </td>
    </tr>
 
    <!-- ===== ORDER DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.SenderOrderId}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Booking Date:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.BookingDate:MMMM d, yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Status:</td>
            <td align=""right"" style=""color:#16A34A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Confirmed
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PACKAGE INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Package Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Product:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.ProductName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Dimensions:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.Dimensions)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Weight:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.Weight)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Value:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{dto.DeclaredValue:N2}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PICKUP DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Pickup Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.PickupAddress)}
              </div>
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Contact:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.ReceiverPhone)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Pickup Date:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.PickupDate:MMMM d, yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Time Slot:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.PickupTime)}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== DELIVERY DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Delivery Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Receiver:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ReceiverName)}
              </div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ReceiverPhone)}
              </div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.DeliveryAddress)}
              </div>
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Est. Delivery:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.EstimatedDeliveryDate:MMMM d, yyyy}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== TRANSPORTER INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Transporter Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Name:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.TransporterName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Contact:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.TransporterPhone)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Vehicle:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.VehicleType)}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PAYMENT SUMMARY ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Payment Summary</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#EFF6FF;border:1px solid #BFDBFE;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Transportation Charge:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{dto.TransportationCharge:N2}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">GST (18%):</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{gst:N2}
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""height:1px;background:#93C5FD;""></div>
            </td>
          </tr>
          <tr>
            <td style=""color:#0A0A0A;font-size:14px;font-weight:600;padding-bottom:6px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Total Paid:</td>
            <td align=""right"" style=""color:#1E3A5F;font-size:14px;font-weight:700;padding-bottom:6px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{totalPaid:N2}
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""color:#585858;font-size:12px;font-weight:400;padding-bottom:2px;
                                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Payment Status: Successful
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""color:#585858;font-size:12px;font-weight:400;
                                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Payment Method: {WebUtility.HtmlEncode(dto.PaymentMethod)}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== TRACK YOUR SHIPMENT ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#F3F4F6;border-radius:10px;padding:16px;"">
          <tr>
            <td>
              <div style=""color:#374151;font-size:14px;font-weight:700;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Track Your Shipment:
              </div>
              <div style=""color:#585858;font-size:14px;font-weight:400;line-height:1.5;margin-bottom:4px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                You can track your shipment status anytime from your dashboard under &quot;Active Orders&quot;.
              </div>
              <div style=""color:#585858;font-size:14px;font-weight:400;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                You&apos;ll receive notifications at each stage: Pickup &#8594; In Transit &#8594; Delivered
              </div>
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== VIEW ORDER BUTTON ===== -->
    <tr>
      <td align=""center"" style=""padding:24px 30px;"">

        <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/active-orders""

           style=""display:inline-block;background:#0C4A6E;color:#fff;border:1px solid #0C4A6E;
                  border-radius:8px;padding:14px 40px;font-size:16px;font-weight:400;
                  text-decoration:none;text-align:center;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          View Order
        </a>
      </td>
    </tr>
 
    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Continue Shopping</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">My Account</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you placed an order with us.
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
          <tr>
            <td align=""center"">
              <a href=""#"" style=""color:#0C4A6E;font-size:12px;font-weight:400;text-decoration:underline;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Unsubscribe from marketing emails
              </a>
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
  </table>
  </td>
</tr>
</table>
 
</body>
</html>";
    }


    // ============================================================
    // METHOD 2: Transporter New Shipment Assignment Email
    // ============================================================
    public async Task SendTransporterAssignmentAsync(
        string email,
        string transporterName,
        SenderOrderConfirmationDto dto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                string body = BuildTransporterShipmentAssignmentTemplate(
                    WebUtility.HtmlEncode(transporterName),
                    dto);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "ITISMYTOWN"),
                    Subject = $"New Shipment Assignment - {dto.SenderOrderId}",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }

            Console.WriteLine($"Transporter assignment email sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending transporter assignment email: {ex.Message}");
            throw new Exception("Failed to send transporter assignment email.");
        }
    }

    private string BuildTransporterShipmentAssignmentTemplate(
        string transporterName,
        SenderOrderConfirmationDto dto)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>New Shipment Assignment - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
 
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"" style=""padding:0;"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px;width:100%;background:#FAFBFC;"">
 
    <!-- ===== HEADER (LOGO) ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>
 
    <!-- ===== BANNER IMAGE ===== -->
    <tr>
      <td align=""center"" style=""padding:0;background:#FAFBFC;"">
        <img src=""https://mytownblobstore.blob.core.windows.net/uploadedfiles/ready_to_deliver.jpeg""
             alt=""Package Ready for Pickup""
             width=""600""
             style=""width:100%;max-width:600px;height:auto;display:block;margin:0 auto;pointer-events:none;"" />
      </td>
    </tr>
 
    <!-- ===== HELLO + INTRO ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {transporterName},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          You have been assigned a new shipment. Please review the details below and ensure timely pickup and delivery.
        </p>
      </td>
    </tr>
 
    <!-- ===== ASSIGNMENT DETAILS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Assignment Details</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.SenderOrderId}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Assignment Date:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.PickupDate:MMMM d, yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Status:</td>
            <td align=""right"" style=""color:#2563EB;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Assigned
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PACKAGE INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Package Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Product:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.ProductName)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Package Type:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.PackageType)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Dimensions:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.Dimensions)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Weight:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.Weight)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Declared Value:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{dto.DeclaredValue:N2}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PICKUP LOCATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#128205; Pickup Location</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#F0FDF4;border:1px solid #BBF7D0;border-radius:10px;padding:16px;"">
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:2px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Sender:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.SenderName)}
              </div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:2px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.PickupAddress)}
              </div>
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Contact:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.SenderPhone)}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Pickup Date:</td>
            <td align=""right"" style=""color:#15803D;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.PickupDate:MMMM d, yyyy}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Time Slot:</td>
            <td align=""right"" style=""color:#15803D;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.PickupTime)}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== DELIVERY LOCATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#128205; Delivery Location</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#FFF7ED;border:1px solid #FED7AA;border-radius:10px;padding:16px;"">
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:2px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Receiver:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ReceiverName)}
              </div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:2px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Contact:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.ReceiverPhone)}
              </div>
            </td>
          </tr>
          <tr>
            <td colspan=""2"" style=""padding-bottom:8px;"">
              <div style=""color:#585858;font-size:14px;font-weight:400;margin-bottom:2px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Address:</div>
              <div style=""color:#0A0A0A;font-size:14px;font-weight:500;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {WebUtility.HtmlEncode(dto.DeliveryAddress)}
              </div>
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Expected Delivery:</td>
            <td align=""right"" style=""color:#C2410C;font-size:14px;font-weight:500;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {dto.EstimatedDeliveryDate:MMMM d, yyyy}
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== PAYMENT INFORMATION ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Payment Information</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid #E5E7EB;border-radius:10px;padding:16px;"">
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Transportation Fee:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &#8377;{dto.TransportationCharge:N2}
            </td>
          </tr>
          <tr>
            <td style=""color:#585858;font-size:14px;font-weight:400;padding-bottom:8px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Payment Method:</td>
            <td align=""right"" style=""color:#0A0A0A;font-size:14px;font-weight:500;padding-bottom:8px;
                                       font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(dto.PaymentMethod)}
            </td>
          </tr>
          <tr>
            <td colspan=""2"">
              <div style=""height:1px;background:#E5E7EB;margin-bottom:8px;""></div>
              <div style=""color:#6B7280;font-size:12px;font-weight:400;line-height:1.5;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                * Payment will be processed by ITISMYTOWN within 48 hours after successful delivery confirmation.
              </div>
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== IMPORTANT INSTRUCTIONS ===== -->
    <tr>
      <td style=""padding:24px 30px;border-bottom:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 16px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#9888;&#65039; Important Instructions</h2>
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#FEFCE8;border:1px solid #FDE68A;border-radius:10px;padding:16px;"">
          <tr>
            <td>
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td width=""12"" valign=""top"" style=""color:#374151;font-size:14px;padding-bottom:8px;padding-right:8px;
                                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8226;</td>
                  <td style=""color:#374151;font-size:14px;font-weight:400;padding-bottom:8px;line-height:1.4;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    Please arrive at the pickup location during the scheduled time slot
                  </td>
                </tr>
                <tr>
                  <td width=""12"" valign=""top"" style=""color:#374151;font-size:14px;padding-bottom:8px;padding-right:8px;
                                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8226;</td>
                  <td style=""color:#374151;font-size:14px;font-weight:400;padding-bottom:8px;line-height:1.4;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    Verify package contents and dimensions before pickup
                  </td>
                </tr>
                <tr>
                  <td width=""12"" valign=""top"" style=""color:#374151;font-size:14px;padding-bottom:8px;padding-right:8px;
                                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8226;</td>
                  <td style=""color:#374151;font-size:14px;font-weight:400;padding-bottom:8px;line-height:1.4;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    Update shipment status in the app: Picked Up &#8594; In Transit &#8594; Delivered
                  </td>
                </tr>
                <tr>
                  <td width=""12"" valign=""top"" style=""color:#374151;font-size:14px;padding-bottom:8px;padding-right:8px;
                                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8226;</td>
                  <td style=""color:#374151;font-size:14px;font-weight:400;padding-bottom:8px;line-height:1.4;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    Handle with care - {WebUtility.HtmlEncode(dto.ProductName)} (Fragile items)
                  </td>
                </tr>
                <tr>
                  <td width=""12"" valign=""top"" style=""color:#374151;font-size:14px;padding-right:8px;
                                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">&#8226;</td>
                  <td style=""color:#374151;font-size:14px;font-weight:400;line-height:1.4;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    Contact receiver before delivery attempt
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
      </td>
    </tr>
 
    <!-- ===== VIEW ORDER BUTTON ===== -->
    <tr>
      <td align=""center"" style=""padding:24px 30px;"">
        <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/transporter/my-plans""
           style=""display:inline-block;background:#0C4A6E;color:#fff;border:1px solid #0C4A6E;
                  border-radius:8px;padding:14px 40px;font-size:16px;font-weight:400;
                  text-decoration:none;text-align:center;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          View Order
        </a>
      </td>
    </tr>
 
    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(139,139,139,0.08);padding:20px 30px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center"" style=""padding-bottom:12px;"">
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Continue Shopping</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">My Account</a>
              <a href=""#"" style=""color:#0C4A6E;font-size:16px;font-weight:400;text-decoration:none;margin:0 12px;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Help Center</a>
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you placed an order with us.
            </td>
          </tr>
          <tr>
            <td align=""center"" style=""color:#585858;font-size:12px;font-weight:400;line-height:1.5;padding-bottom:6px;
                                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
          <tr>
            <td align=""center"">
              <a href=""#"" style=""color:#0C4A6E;font-size:12px;font-weight:400;text-decoration:underline;
                                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Unsubscribe from marketing emails
              </a>
            </td>
          </tr>
        </table>
      </td>



    </tr>
 
  </table>
  </td>
</tr>
</table>
 
</body>
</html>";
}


    public async Task SendGuestNotificationforTracking(
    string email,
    string guestName,
    OrderConfirmationDto orderdto)
    {
        if (!await DomainHasMX(email))
            throw new Exception("The email domain is not valid (no MX records found).");

        try
        {
            var htmlBody = BuildGuestTrackingTemplate(
                WebUtility.HtmlEncode(guestName),
                orderdto);

        
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
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

            Console.WriteLine($"Guest order confirmation email sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending guest notification email: {ex.Message}");
            throw new Exception("Failed to send guest notification email.");
        }
    }

    private string BuildGuestTrackingTemplate(string guestName, OrderConfirmationDto orderdto)
{
    var storesBuilder = new StringBuilder();
    var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";

    foreach (var store in orderdto.Stores)
    {
        var productsBuilder = new StringBuilder();

        foreach (var item in store.Items)
        {
            string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                ? "https://via.placeholder.com/80x80?text=No+Image"
                : item.ImageUrl;

            productsBuilder.Append($@"
<!-- Product Card -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""border:1px solid #E5E7EB;border-radius:12px;margin-bottom:12px;"">
  <tr>
    <td style=""padding:12px;"">
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
          <td width=""80"" valign=""top"" style=""padding-right:12px;"">
            <img src=""{imageBaseUrl}/{Uri.EscapeDataString(imageSrc)}""
                 alt=""Product"" width=""80"" height=""80""
                 style=""width:80px;height:80px;border-radius:8px;object-fit:cover;display:block;background:#F5F5F5;"" />
          </td>
          <td valign=""top"">
            <div style=""color:#52525B;font-size:14px;font-weight:600;line-height:20px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.ProductName)}
            </div>
            <div style=""color:#9CA3AF;font-size:12px;font-weight:400;line-height:16px;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(item.Productdesc ?? string.Empty)}
            </div>
          </td>
        </tr>
      </table>
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:12px;"">
        <tr>
          <td style=""color:#52525B;font-size:14px;font-weight:500;
                      font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            Qty: {item.Quantity}
          </td>
          <td align=""right"" style=""color:#52525B;font-size:16px;font-weight:600;
                                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
            &#8377;{item.ItemTotal:F2}
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>");
        }

        storesBuilder.Append($@"
<!-- ===== STORE BLOCK ===== -->
<!-- Tracking Card -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#DBEAFE;border-radius:12px;padding:24px;margin-bottom:16px;
              box-shadow:0px 12px 24px -4px rgba(12,71,131,0.08);"">
  <tr>
    <td>
      <!-- Top row: tracking label + status badge -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-bottom:16px;"">
        <tr>
          <td>
            <div style=""color:#1E3A5F;font-size:14px;font-weight:600;margin-bottom:8px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Tracking Information
            </div>
            <table cellpadding=""0"" cellspacing=""0"" border=""0"">
              <tr>
                <td style=""color:#52525B;font-size:14px;font-weight:500;padding-right:8px;
                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                  Tracking ID:
                </td>
                <td style=""color:#000;font-size:16px;font-weight:600;
                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                  {store.TrackingId}
                </td>
                <td style=""padding-left:12px;"">
                  <div style=""background:#14532D;color:#fff;font-size:10px;font-weight:500;
                               padding:2px 10px;border-radius:99px;letter-spacing:0.05em;text-transform:uppercase;
                               font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                    ORDER CONFIRMED
                  </div>
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>

      <!-- Track Shipment button -->     
<a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/track-order?id={store.TrackingId}""
   style=""display:block;background:#0E7490;color:#fff;text-align:center;
                padding:10px 0;border-radius:6px;font-size:16px;font-weight:600;
                text-decoration:none;margin-bottom:8px;
                font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        Track Shipment
      </a>
      <div style=""text-align:center;color:#52525B;font-size:12px;font-weight:500;
                  font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        Use this tracking ID to track your shipment anytime.
      </div>
    </td>
  </tr>
</table>

<!-- Store Information -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
              border-radius:8px;padding:24px;margin-bottom:16px;"">
  <tr>
    <td>
      <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Information</div>
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
             style=""border:1px solid rgba(0,0,0,0.10);border-radius:8px;padding:16px;"">
        <tr>
          <td>
            <div style=""color:#000;font-size:20px;font-weight:600;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(store.StoreName)}
            </div>
            <div style=""color:#9CA3AF;font-size:12px;font-weight:500;margin-bottom:16px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(store.StoreAddress ?? string.Empty)}
            </div>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
              <tr>
                <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:10px;
                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Order ID</td>
               <td align=""right"" style=""padding-bottom:10px;"">
                    <span style=""color:#004481;font-size:14px;font-weight:600;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                        {store.StoreOrderId}
                    </span>
                </td>
              </tr>
              <tr>
                <td style=""color:#6B7280;font-size:14px;font-weight:500;padding-bottom:10px;
                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Phone Number</td>
                <td align=""right"" style=""color:#52525B;font-size:14px;font-weight:600;padding-bottom:10px;
                                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                  {WebUtility.HtmlEncode(store.BusinessPhone ?? string.Empty)}
                </td>
              </tr>
              <tr>
                <td style=""color:#6B7280;font-size:14px;font-weight:500;
                            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Email</td>
                <td align=""right"" style=""color:#52525B;font-size:14px;font-weight:600;
                                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                  {WebUtility.HtmlEncode(store.BusinessEmail ?? string.Empty)}
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>

<!-- Products to Deliver -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
              border-radius:8px;padding:24px;margin-bottom:16px;"">
  <tr>
    <td>
      <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Products to Deliver</div>
      {productsBuilder}
      <!-- Expected Delivery Date -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
          <td style=""background:#F0FFF4;border:1px solid #BBF7D0;border-radius:8px;padding:12px 16px;"">
            <div style=""color:#14532D;font-size:12px;font-weight:600;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Expected Delivery Date</div>
            <div style=""color:#14532D;font-size:12px;font-weight:400;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              Please deliver by {store.EstimatedDeliveryDate:MMMM dd, yyyy}
            </div>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>

<!-- Customer Information -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
              border-radius:8px;padding:24px;margin-bottom:16px;"">
  <tr>
    <td>
      <div style=""color:#000;font-size:18px;font-weight:500;margin-bottom:16px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Customer Information</div>
      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
             style=""border:1px solid rgba(0,0,0,0.10);border-radius:12px;padding:12px;"">
        <tr>
          <td style=""padding-bottom:12px;"">
            <div style=""color:#000;font-size:16px;font-weight:600;margin-bottom:4px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(orderdto.ShopperName)}
            </div>
            <div style=""color:#6B7280;font-size:14px;font-weight:500;margin-bottom:2px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(orderdto.ShopperPhone)}
            </div>
            <div style=""color:#6B7280;font-size:14px;font-weight:500;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(orderdto.ShopperEmail ?? string.Empty)}
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div style=""color:#52525B;font-size:14px;font-weight:400;line-height:1.5;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              {WebUtility.HtmlEncode(orderdto.DeliveryAddress)}
            </div>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>");
    }

    return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Order Tracking - ITISMYTOWN</title>
</head>
<body style=""margin:0;padding:0;background:#FAFBFC;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#FAFBFC"">
<tr>
  <td align=""center"">
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
         style=""max-width:600px;width:100%;background:#FAFBFC;"">

    <!-- ===== HEADER ===== -->
    <tr>
      <td align=""center"" style=""padding:20px 30px;border-bottom:1px solid #F1F1F3;background:#fff;"">
        <img src=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <!-- ===== BANNER (gradient bg + shipped icon card) ===== -->
    <tr>
      <td align=""center""
          style=""padding:48px 28px;background:linear-gradient(to bottom,#155E75,#ffffff);"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border-radius:8px;padding:24px;text-align:center;"">
          <tr>
            <td align=""center"">
              <!-- Green circle icon -->
              <div style=""width:48px;height:48px;margin:0 auto 12px;
                           background:linear-gradient(to bottom,#14532D,#166534);
                           border-radius:50%;border:3px solid rgba(20,83,45,0.10);
                           display:inline-block;"">
                &nbsp;
              </div>
              <div style=""color:#1E293B;font-size:24px;font-weight:600;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Your Order is On the Way
              </div>
              <div style=""color:#71717A;font-size:12px;font-weight:400;line-height:20px;max-width:384px;margin:0 auto;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Your order has been successfully placed and a tracking ID has been generated for your shipment.
              </div>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ===== HELLO + ORDER ID ROW ===== -->
    <tr>
      <td style=""padding:20px 28px;border-bottom:1px solid #fff;"">
        <p style=""color:#000;font-size:16px;font-weight:700;line-height:1.5;margin:0 0 8px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Hello {guestName},
        </p>
        <p style=""color:#000;font-size:16px;font-weight:400;line-height:1.5;margin:0 0 16px 0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Thank you for your purchase. You can use the tracking information below to monitor your shipment status and delivery progress.
        </p>

        <!-- Order ID / Date row -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border:1px solid rgba(113,113,122,0.10);
                      border-radius:4px;padding:24px;margin-bottom:16px;"">
          <tr>
            <td style=""padding-right:16px;"">
              <div style=""color:#52525B;font-size:14px;font-weight:500;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>
              <div style=""color:#52525B;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
              </div>
            </td>
            <td>
              <div style=""color:#52525B;font-size:14px;font-weight:500;margin-bottom:8px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#52525B;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
          </tr>
        </table>

        <!-- Per-store: tracking card + store info + products + customer -->
        {storesBuilder}

        <!-- ===== CREATE ACCOUNT CTA ===== -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#FFFBEB;border:1px solid #FDE68A;border-radius:12px;
                      padding:24px;margin-bottom:0;
                      box-shadow:0px 12px 24px -4px rgba(12,71,131,0.08);"">
          <tr>
            <td>
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-bottom:16px;"">
                <tr>
                  <td width=""40"" valign=""top"" style=""padding-right:16px;"">
                    <div style=""width:40px;height:40px;background:#FEF3C7;border-radius:50%;
                                 display:inline-block;"">
                      &nbsp;
                    </div>
                  </td>
                  <td valign=""top"">
                    <div style=""color:#78350F;font-size:18px;font-weight:700;margin-bottom:4px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Create Your ITISMYTOWN Account
                    </div>
                    <div style=""color:rgba(120,53,15,0.80);font-size:14px;font-weight:400;margin-bottom:12px;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                      Unlock exclusive benefits by registering today:
                    </div>
                    <!-- Benefit bullets -->
                    <table cellpadding=""0"" cellspacing=""0"" border=""0"">
                      <tr>
                        <td valign=""top"" style=""padding-right:8px;color:rgba(120,53,15,0.90);font-size:14px;"">&#8226;</td>
                        <td style=""color:rgba(120,53,15,0.90);font-size:12px;font-weight:400;padding-bottom:6px;line-height:16px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Save multiple addresses for faster checkout
                        </td>
                      </tr>
                      <tr>
                        <td valign=""top"" style=""padding-right:8px;color:rgba(120,53,15,0.90);font-size:14px;"">&#8226;</td>
                        <td style=""color:rgba(120,53,15,0.90);font-size:12px;font-weight:400;padding-bottom:6px;line-height:16px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Earn &lsquo;Town Points&rsquo; on every purchase
                        </td>
                      </tr>
                      <tr>
                        <td valign=""top"" style=""padding-right:8px;color:rgba(120,53,15,0.90);font-size:14px;"">&#8226;</td>
                        <td style=""color:rgba(120,53,15,0.90);font-size:12px;font-weight:400;line-height:16px;
                                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                          Real-time mobile push notifications for delivery
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
              <!-- Create Account button -->
              <a href=""https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net/register""
                 style=""display:block;background:#0E7490;color:#fff;text-align:center;
                        padding:12px 0;border-radius:6px;font-size:16px;font-weight:700;
                        text-decoration:none;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Create Account
              </a>
            </td>
          </tr>
        </table>

      </td>
    </tr>

    <!-- ===== FOOTER ===== -->
    <tr>
      <td style=""background:rgba(113,113,122,0.10);padding:20px 28px 24px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
          <tr>
            <td align=""center""
                style=""color:#52525B;font-size:12px;font-weight:400;line-height:1.5;
                        padding-bottom:6px;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              You&#39;re receiving this email because you placed an order with us.
            </td>
          </tr>
          <tr>
            <td align=""center""
                style=""color:#52525B;font-size:12px;font-weight:400;line-height:1.5;
                        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
              &copy; 2026 itismytown. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>

  </table>
  </td>
</tr>
</table>

</body>
</html>";
}


}
