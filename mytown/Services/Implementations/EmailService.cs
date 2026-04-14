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

    // order Email confirmation to business owner
    //    public async Task SendBusinessnotificationforOrderCnf(
    //      string email,
    //      string businessname,
    //      OrderConfirmationDto orderdto,
    //      StoreOrderConfirmationDto storedto)
    //    {
    //        if (!await DomainHasMX(email))
    //            throw new Exception("The email domain is not valid (no MX records found).");

    //        try
    //        {
    //            using (var smtpClient = new SmtpClient(_smtpServer))
    //            {
    //                smtpClient.Port = _smtpPort;
    //                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
    //                smtpClient.EnableSsl = true;

    //                var mailMessage = new MailMessage
    //                {
    //                    From = new MailAddress(_senderEmail),
    //                    Subject = $"New Order Received - {storedto.StoreOrderId}",
    //                    IsBodyHtml = true
    //                };

    //                mailMessage.To.Add(email);

    //                // =============================
    //                // 🔹 BUILD DYNAMIC ITEMS HTML
    //                // =============================

    //                var itemsHtml = new StringBuilder();
    //                var imageBaseUrl = "https://mytownblobstore.blob.core.windows.net/uploadedfiles";

    //                foreach (var item in storedto.Items)
    //                {
    //                   itemsHtml.Append($@"
    //<tr>
    //    <td style=""border-radius: 12px; border: 1px solid #E5E7EB; padding: 12px 16px; margin-bottom: 12px;"">
    //        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    //            <tr>
    //                <td style=""width: 48px; padding-right: 16px;"">
    //                    <img src=""{imageBaseUrl}/{Uri.EscapeDataString(item.ImageUrl)}""
    //                         style=""width: 48px; height: 48px; border-radius: 8px; object-fit: cover;"" />
    //                </td>
    //                <td style=""flex: 1; padding-right: 16px;"">
    //                    <p style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4; margin: 0;"">
    //                        {item.ProductName}
    //                    </p>
    //                </td>
    //                <td style=""text-align: right;"">
    //                    <span style=""color: #585858; font-size: 14px; font-weight: 500;"">
    //                        Qty: {item.Quantity}
    //                    </span>
    //                    <span style=""color: #585858; font-size: 14px; font-weight: 600; margin-left: 16px;"">
    //                        ₹{item.ItemTotal:N2}
    //                    </span>
    //                </td>
    //            </tr>
    //        </table>
    //    </td>
    //</tr>");
    //                }

    //                // =============================
    //                // 🔹 FULL EMAIL BODY (EXACT DESIGN)
    //                // =============================

    //                var body = $@"
    //<!DOCTYPE html>
    //<html lang=""en"">
    //<head>
    //<meta charset=""UTF-8"">
    //<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    //<title>New Order Received - ITISMYTOWN</title>
    //</head>

    //<body style=""margin: 0; padding: 0; font-family: -apple-system, Roboto, Helvetica, sans-serif; background-color: #f3f4f6; padding: 24px 16px; display: flex; justify-content: center; min-height: 100vh;"">

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width: 600px; background-color: #FAFBFC; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);"">

    //<tr>
    //<td style=""padding: 20px 30px; border-bottom: 1px solid #F1F1F3; background-color: #fff; text-align: center;"">
    //<img src=""https://api.builder.io/api/v1/image/assets/TEMP/61706e9c591f3c915cc46d92b1a6a96e3c9c3a70?width=462"" alt=""ITISMYTOWN"" style=""height: 46px; width: auto;"">
    //</td>
    //</tr>

    // <!-- Hero Section -->
    //        <tr>
    //            <td style=""padding: 48px 24px; background: linear-gradient(180deg, #285A8C 0%, #fff 100%); text-align: center;"">
    //                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; padding: 32px 24px; position: relative;"">
    //                    <!-- Decorative dots (simplified) -->
    //                    <tr>
    //                        <td style=""position: relative;"">
    //                            <div style=""position: absolute; background-color: #F1F1F3; border-radius: 50%; width: 5px; height: 5px; top: 24px; right: 48px;""></div>
    //                            <div style=""position: absolute; background-color: #F1F1F3; border-radius: 50%; width: 8px; height: 8px; top: 32px; left: 40px;""></div>
    //                            <div style=""position: absolute; background-color: #F1F1F3; border-radius: 50%; width: 4px; height: 4px; top: 56px; left: 48px;""></div>
    //                            <div style=""position: absolute; background-color: #F1F1F3; border-radius: 50%; width: 8px; height: 8px; top: 64px; left: 36px;""></div>

    //                            <!-- Checkmark Icon -->
    //                           <div style=""width: 64px; height: 64px; border-radius: 50%; border: 4px solid rgba(16, 86, 23, 0.1); background: linear-gradient(180deg, #105617 0%, #2F7C37 100%); display: inline-flex; align-items: center; justify-content: center; margin: 0 auto;"">
    //                            <svg viewBox=""0 0 32 32"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"" style=""width: 32px; height: 32px;"">
    //                                <path d=""M2.66699 15.9023L11.3809 24.63L29.3337 6.66797"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""/>
    //                            </svg>
    //                        </div>


    //                            <!-- Confirmation Text -->
    //                            <div style=""margin-top: 24px; text-align: center;"">
    //                                <h1 style=""color: #182D41; font-size: 28px; font-weight: 600; line-height: 1; margin: 0;"">New Order Received!</h1>
    //                                <p style=""color: #7A7A7A; font-size: 16px; font-weight: 400; line-height: 1.5; max-width: 400px; margin: 8px auto 0;"">You have received a new order. Please review and process it as soon as possible.</p>
    //                            </div>
    //                        </td>
    //                    </tr>
    //                </table>
    //            </td>
    //        </tr>


    //<tr>
    //<td style=""padding: 20px 30px 24px;"">

    //<div style=""margin-bottom: 16px;"">
    //<p style=""color: #000; font-size: 16px; font-weight: 700; line-height: 1.5; margin: 0 0 12px 0;"">
    //Hello {storedto.StoreName},
    //</p>
    //<p style=""color: #000; font-size: 16px; font-weight: 400; line-height: 1.5; margin: 0;"">
    //You have received a new order from {orderdto.ShopperName}. Please prepare the items for shipment.
    //</p>
    //</div>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 4px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    //<tr>

    //<td style=""padding: 0 16px 16px 0;"">
    //<div style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Order ID</div>
    //<div style=""color: #585858; font-size: 16px; font-weight: 600; line-height: 1.25;"">
    //ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
    //</div>
    //</td>

    //<td style=""padding: 0 16px 16px 0;"">
    //<div style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Store Order ID</div>
    //<div style=""color: #585858; font-size: 16px; font-weight: 600; line-height: 1.25;"">
    //{storedto.StoreOrderId}
    //</div>
    //</td>

    //<td style=""padding: 0 16px 16px 0;"">
    //<div style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Order Date</div>
    //<div style=""color: #585858; font-size: 16px; font-weight: 600; line-height: 1.25;"">
    //{orderdto.OrderDate:MMMM dd, yyyy}
    //</div>
    //</td>

    //<td style=""padding: 0;"">
    //<div style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Total Amount</div>
    //<div style=""color: #585858; font-size: 16px; font-weight: 600; line-height: 1.25;"">
    //₹{storedto.StoreTotal:N2}
    //</div>
    //</td>

    //</tr>
    //</table>

    //</td>
    //</tr>
    //</table>

    //   <!-- Customer Information -->
    //                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //                    <tr>
    //                        <td>
    //                            <h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Customer Information</h2>
    //                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-radius: 12px; border: 1px solid #E5E7EB; padding: 20px;"">
    //                                <tr>
    //                                    <td style=""padding-bottom: 16px;"">
    //                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    //                                            <tr>
    //                                                <td style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Customer Name</td>
    //                                                <td style=""color: #585858; font-size: 14px; font-weight: 600; line-height: 1.4; text-align: right;"">{orderdto.ShopperName}</td>
    //                                            </tr>
    //                                        </table>
    //                                    </td>
    //                                </tr>
    //                                <tr>
    //                                    <td style=""border-top: 1px solid #E5E7EB; padding-top: 16px;"">
    //                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    //                                            <tr>
    //                                                <td style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">Phone Number</td>
    //                                                <td style=""color: #585858; font-size: 14px; font-weight: 600; line-height: 1.4; text-align: right;"">{orderdto.ShopperPhone}</td>
    //                                            </tr>
    //                                        </table>
    //                                    </td>
    //                                </tr>
    //                            </table>
    //                        </td>
    //                    </tr>
    //                </table>


    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Items Ordered</h2>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    //{itemsHtml}
    //</table>

    //</td>
    //</tr>
    //</table>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Payment Method</h2>
    //<p style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4; margin: 0;"">
    //{orderdto.PaymentMethod}
    //</p>
    //<p style=""color: #22A048; font-size: 14px; font-weight: 600; line-height: 1.4; margin: 4px 0 0 0;"">
    //Payment successful
    //</p>
    //</td>
    //</tr>
    //</table>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Estimated Delivery Date</h2>
    //<p style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4; margin: 0;"">
    //{storedto.EstimatedDeliveryDate:MMMM dd, yyyy}
    //</p>
    //</td>
    //</tr>
    //</table>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Shipping method</h2>
    //<p style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4; margin: 0;"">
    //{storedto.ShippingType}
    //</p>
    //</td>
    //</tr>
    //</table>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #fff; border-radius: 8px; border: 1px solid rgba(139, 139, 139, 0.08); padding: 24px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<h2 style=""color: #000; font-size: 18px; font-weight: 500; line-height: 1; margin: 0 0 16px 0;"">Delivery Address</h2>
    //<div style=""margin-bottom: 8px;"">
    //<p style=""color: #585858; font-size: 14px; font-weight: 600; line-height: 1.4; margin: 0;"">
    //{orderdto.ShopperName}
    //</p>
    //<p style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4; margin: 4px 0 0 0;"">
    //{orderdto.DeliveryAddress}
    //</p>
    //</div>
    //<div style=""display: flex; align-items: center; gap: 8px;"">
    //<div style=""width: 24px; height: 24px; border-radius: 50%; background-color: #F5F5F5; display: flex; align-items: center; justify-content: center;"">
    //<img src=""https://api.builder.io/api/v1/image/assets/TEMP/1fe9cca9f23167203c9ed4a6d5e4c3fb69278262?width=28"" style=""width: 12px; height: 12px;"" />
    //</div>
    //<span style=""color: #585858; font-size: 14px; font-weight: 500; line-height: 1.4;"">
    //{orderdto.ShopperPhone}
    //</span>
    //</div>
    //</td>
    //</tr>
    //</table>

    //<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-radius: 8px; border: 1px solid #F7BBBB; background-color: #FDF0F0; padding: 12px 16px; margin-bottom: 16px;"">
    //<tr>
    //<td>
    //<p style=""color: #FF1434; font-size: 14px; line-height: 1.5; margin: 0;"">
    //<strong style=""display: block; margin-bottom: 4px;"">Action Required</strong>
    //Please process this order and update the shipping status. The customer is expecting delivery by {storedto.EstimatedDeliveryDate:MMMM dd, yyyy}.
    //</p>
    //</td>
    //</tr>
    //</table>
    //  <!-- Button -->
    //                <div style=""text-align: center; margin-top: 8px;"">
    //                    <a href=""#"" style=""display: inline-block; height: 48px; padding: 0 32px; background-color: #004481; border: 1px solid #004481; border-radius: 8px; color: #fff; font-size: 16px; font-weight: 400; text-decoration: none; line-height: 48px; font-family: -apple-system, Roboto, Helvetica, sans-serif;"">View Order Details</a>
    //                </div>
    //            </td>
    //        </tr>

    //        <!-- Footer -->
    //        <tr>
    //            <td style=""background-color: rgba(139, 139, 139, 0.08); padding: 20px 30px 24px; text-align: center;"">
    //                <div style=""margin-bottom: 12px;"">
    //                    <a href=""#"" style=""color: #004481; text-decoration: none; font-size: 16px; font-weight: 400; margin: 0 12px;"">Seller Dashboard</a>
    //                    <a href=""#"" style=""color: #004481; text-decoration: none; font-size: 16px; font-weight: 400; margin: 0 12px;"">Order Management</a>
    //                    <a href=""#"" style=""color: #004481; text-decoration: none; font-size: 16px; font-weight: 400; margin: 0 12px;"">Help Center</a>
    //                </div>
    //                <p style=""color: #585858; font-size: 12px; line-height: 1.5; margin: 0;"">You're receiving this email because you're a seller on our platform.</p>
    //                <p style=""color: #585858; font-size: 12px; line-height: 1.5; margin: 8px 0 0 0;"">© 2026 itismytown. All rights reserved.</p>
    //            </td>
    //        </tr>

    //</table>
    //</body>
    //</html>";

    //                mailMessage.Body = body;

    //                await smtpClient.SendMailAsync(mailMessage);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error sending business notification email: {ex.Message}");
    //            throw new Exception("Failed to send business notification email.");
    //        }
    //    }


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
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
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
        <img src=""https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <!-- ===== HERO GRADIENT SECTION ===== -->
    <tr>
      <td style=""padding:8px 30px 0px 30px;background:linear-gradient(180deg,#285A8C 0%,#ffffff 100%);"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border-radius:8px;padding:10px 24px;"">
          <tr>
            <td align=""center"" style=""padding-bottom:20px;"">

              <!-- ✅ GREEN CIRCLE WITH TICK — EMAIL-SAFE TABLE LAYOUT -->
              <table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin:0 auto 20px auto;"">
                <tr>
                  <td width=""72"" height=""72"" align=""center"" valign=""middle""
                      bgcolor=""#105617""
                      style=""width:72px;height:72px;border-radius:50%;
                             background:linear-gradient(180deg,#105617 0%,#2F7C37 100%);
                             border:4px solid rgba(16,86,23,0.10);
                             text-align:center;vertical-align:middle;"">
                    <!--[if mso]>
                    <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml""
                      xmlns:w=""urn:schemas-microsoft-com:office:word""
                      style=""height:72px;v-text-anchor:middle;width:72px;""
                      arcsize=""50%"" strokecolor=""transparent"" fillcolor=""#105617"">
                    <w:anchorlock/>
                    <center>
                    <![endif]-->
                    <img src=""https://img.icons8.com/ios/50/ffffff/checkmark--v1.png""
                         alt=""Order Confirmed"" width=""32"" height=""32""
                         style=""width:32px;height:32px;display:block;margin:0 auto;"" />
                    <!--[if mso]></center></v:roundrect><![endif]-->
                  </td>
                </tr>
              </table>

              <!-- Heading -->
              <h1 style=""color:#182D41;font-size:22px;font-weight:600;text-align:center;
                          margin:0 0 10px 0;
                          font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                New Order Received!
              </h1>
              <p style=""color:#7A7A7A;font-size:16px;font-weight:400;text-align:center;
                         line-height:1.5;margin:0;max-width:420px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                You have received a new order. Please review and process it as soon as possible.
              </p>

            </td>
          </tr>
        </table>
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
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:6px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order ID</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}
              </div>
            </td>
            <td style=""padding-right:12px;"">
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:6px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Store Order ID</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {storedto.StoreOrderId}
              </div>
            </td>
            <td style=""padding-right:12px;"">
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:6px;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Order Date</div>
              <div style=""color:#585858;font-size:16px;font-weight:600;
                           font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                {orderdto.OrderDate:MMMM dd, yyyy}
              </div>
            </td>
            <td>
              <div style=""color:#585858;font-size:14px;font-weight:500;margin-bottom:6px;
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
        {storedto.ShippingType}
      </div>

      <!-- Divider -->
      <div style=""height:1px;background:#F1F1F3;margin:16px 0;""></div>

      <!-- Courier Info -->
      <div style=""color:#585858;font-size:13px;font-weight:500;margin-bottom:4px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        Courier Name
      </div>
      <div style=""color:#000;font-size:15px;font-weight:600;margin-bottom:12px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        {storedto.CourierName}
      </div>

      <div style=""color:#585858;font-size:13px;font-weight:500;margin-bottom:4px;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        Courier Phone
      </div>
      <div style=""color:#000;font-size:15px;font-weight:600;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
        {storedto.CourierPhone}
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
              <a href=""#""
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
        <td align=""right"" style=""color:#585858;font-size:16px;font-weight:500;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;padding-bottom:6px;"">{store.StoreOrderId}</td>
      </tr>
      <tr>
        <td style=""color:#000;font-size:16px;font-weight:400;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Estimated Date of Delivery</td>
        <td align=""right"" style=""color:#585858;font-size:16px;font-weight:500;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">{store.EstimatedDeliveryDate:MMMM dd, yyyy}</td>
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
        <img src=""https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png""
             alt=""ITISMYTOWN"" height=""55""
             style=""height:55px;width:auto;display:block;margin:0 auto;"" />
      </td>
    </tr>

    <!-- ===== HERO GRADIENT SECTION ===== -->
    <tr>
      <td style=""padding:8px 30px 0px 30px;background:linear-gradient(180deg,#285A8C 0%,#ffffff 100%);"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
               style=""background:#fff;border-radius:8px;padding:10px 24px;"">
          <tr>
            <td align=""center"">

              <!-- ✅ GREEN CIRCLE WITH TICK — TABLE BASED (email-safe) -->
              <table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin:0 auto;"">
                <tr>
                  <td width=""48"" height=""48"" align=""center"" valign=""middle""
                      style=""width:48px;height:48px;border-radius:50%;
                             background:linear-gradient(180deg,#105617 0%,#2F7C37 100%);
                             border:4px solid rgba(16,86,23,0.10);
                             text-align:center;vertical-align:middle;"">
                    <!--[if mso]>
                    <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml""
                      xmlns:w=""urn:schemas-microsoft-com:office:word""
                      style=""height:64px;v-text-anchor:middle;width:64px;""
                      arcsize=""50%"" strokecolor=""transparent""
                      fillcolor=""#105617"">
                    <w:anchorlock/>
                    <center>
                    <![endif]-->
                    <img src=""https://img.icons8.com/ios/50/ffffff/checkmark--v1.png""
                         alt=""✓"" width=""22"" height=""22""
                         width:22px;height:22px;display:block;margin:0 auto;"" />
                    <!--[if mso]></center></v:roundrect><![endif]-->
                  </td>
                </tr>
              </table>

              <!-- Heading -->
              <h1 style=""color:#182D41;font-size:22px;font-weight:600;text-align:center;
                          margin:10px 0 6px 0;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Order Confirmed!
              </h1>
              <p style=""color:#7A7A7A;font-size:16px;font-weight:400;text-align:center;
                         line-height:1.5;margin:0 0 4px 0;max-width:400px;
                         font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
                Thank you for your purchase! Your order has been successfully placed.
              </p>

            </td>
          </tr>
        </table>
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

    <!-- ===== SHIPPING METHOD ===== -->
    <tr>
      <td style=""padding:24px 30px;border-top:1px solid #F1F1F3;"">
        <h2 style=""color:#000;font-size:18px;font-weight:500;margin:0 0 12px 0;
                    font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">Shipping method</h2>
        <p style=""color:#585858;font-size:14px;font-weight:500;margin:0;
                   font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;"">
          Standard
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
        <a href=""https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/orders/{WebUtility.HtmlEncode(orderdto.OrderId.ToString())}""
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

    //private string BuildShopperNotificationTemplate(string shopperName, OrderConfirmationDto orderdto)
    //{
    //    var storesBuilder = new StringBuilder();

    //    foreach (var store in orderdto.Stores)
    //    {
    //        var itemsBuilder = new StringBuilder();

    //        foreach (var item in store.Items)
    //        {
    //            string imageSrc = string.IsNullOrEmpty(item.ImageUrl) ? "https://via.placeholder.com/64x64?text=No+Image" : item.ImageUrl;
    //            itemsBuilder.Append($@"
    //<div style='display: flex; align-items: center; gap: 8px; padding-bottom: 12px; border-bottom: 1px solid #E5E7EB;'>
    //    <img src='{imageSrc}' alt='Product' style='width: 64px; height: 64px; border-radius: 6px; object-fit: cover; flex-shrink: 0;' />
    //    <div style='flex: 1; min-width: 0; display: flex; justify-content: space-between; align-items: center; gap: 8px;'>
    //        <span style='color: #585858; font-size: 14px; font-weight: 600; line-height: 20px; flex: 1;'>{WebUtility.HtmlEncode(item.ProductName)}</span>
    //        <span style='color: #585858; font-size: 14px; font-weight: 500; white-space: nowrap;'>Qty: {item.Quantity}</span>
    //        <span style='color: #585858; font-size: 14px; font-weight: 600; white-space: nowrap;'>₹{item.ItemTotal:F2}</span>
    //    </div>
    //</div>");
    //        }

    //        storesBuilder.Append($@"
    //<div style='background: #FFF; padding: 20px 24px; border-bottom: 1px solid #E5E7EB;'>
    //    <div style='display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 12px; gap: 16px; flex-wrap: wrap;'>
    //        <div>
    //            <h3 style='color: #000; font-size: 20px; font-weight: 600; margin-bottom: 4px;'>{WebUtility.HtmlEncode(store.StoreName)}</h3>
    //        </div>
    //        <div style='display: flex; flex-direction: column; align-items: flex-end; gap: 8px; text-align: right;'>
    //            <span style='color: #000; font-size: 16px; font-weight: 400;'>Store Total</span>
    //            <span style='color: #585858; font-size: 16px; font-weight: 700;'>₹{store.StoreTotal:F2}</span>
    //        </div>
    //    </div>

    //    <div style='display: flex; flex-direction: column; gap: 8px; margin-bottom: 12px;'>
    //        <div style='display: flex; justify-content: space-between; align-items: center;'>
    //            <span style='color: #000; font-size: 16px; font-weight: 400;'>Store Order ID</span>
    //            <span style='color: #585858; font-size: 16px; font-weight: 500; text-align: right;'>{store.StoreOrderId}</span>
    //        </div>
    //        <div style='display: flex; justify-content: space-between; align-items: center;'>
    //            <span style='color: #000; font-size: 16px; font-weight: 400;'>Estimated Date of Delivery</span>
    //            <span style='color: #585858; font-size: 16px; font-weight: 500; text-align: right;'>{store.EstimatedDeliveryDate:MMMM dd, yyyy}</span>
    //        </div>
    //    </div>

    //    <h4 style='color: #000; font-size: 18px; font-weight: 500; margin-bottom: 12px; margin-top: 12px;'>Items in your order</h4>
    //    <div style='border: 1px solid #E5E7EB; border-radius: 12px; padding: 12px; display: flex; flex-direction: column; gap: 16px; margin-bottom: 12px;'>
    //        {itemsBuilder}
    //    </div>

    //    <div style='background: #F0FFF4; border: 1px solid #BBF7D0; border-radius: 8px; padding: 12px; margin-bottom: 12px;'>
    //        <p style='color: #166534; font-size: 14px; font-weight: 500; line-height: 1.5; margin: 0;'>
    //            <strong>{WebUtility.HtmlEncode(store.ShippingStatus)}</strong><br/>
    //            Expected delivery by {store.EstimatedDeliveryDate:MMMM dd, yyyy}
    //        </p>
    //    </div>
    //</div>");
    //    }

    //    // Assuming subtotal is total amount, shipping and tax are included or 0
    //    decimal subtotal = orderdto.TotalAmount;
    //    decimal shipping = 0; // Placeholder
    //    decimal tax = 0; // Placeholder

    //    return $@"
    //<!DOCTYPE html>
    //<html lang='en'>
    //<head>
    //    <meta charset='UTF-8'>
    //    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    //    <title>Order Confirmation - ITISMYTOWN</title>
    //</head>
    //<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, sans-serif; line-height: 1.5; background: #FAFBFC;'>

    //<div style='max-width: 600px; margin: 0 auto; background: #FAFBFC;'>

    //    <!-- Header -->
    //    <div style='padding: 20px 30px; border-bottom: 1px solid #F1F1F3; display: flex; align-items: center;'>
    //        <img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' alt='ITISMYTOWN' style='height: 55px; width: auto;' />
    //    </div>

    //    <!-- Hero Section -->
    //    <div style='padding: 48px 30px; background: linear-gradient(180deg, #285A8C 0%, #FFF 100%); display: flex; flex-direction: column; align-items: center; gap: 16px;'>
    //        <div style='background: #FFF; border-radius: 8px; padding: 24px; width: 100%; display: flex; flex-direction: column; align-items: center; gap: 24px;'>
    //            <div style='width: 64px; height: 64px; border-radius: 100px; background: linear-gradient(180deg, #105617 0%, #2F7C37 100%); border: 4px solid rgba(16, 86, 23, 0.10); display: flex; align-items: center; justify-content: center; flex-shrink: 0;'>
    //                <svg width='32' height='32' viewBox='0 0 32 32' fill='none' xmlns='http://www.w3.org/2000/svg' style='width: 32px; height: 32px;'>
    //                    <path d='M2.66699 15.9023L11.3809 24.63L29.3337 6.66797' stroke='white' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'/>
    //                </svg>
    //            </div>
    //            <div style='display: flex; flex-direction: column; align-items: center; gap: 8px; width: 100%;'>
    //                <h1 style='color: #182D41; font-size: 28px; font-weight: 600; text-align: center; margin: 0;'>Order Confirmed!</h1>
    //                <p style='color: #7A7A7A; font-size: 16px; font-weight: 400; text-align: center; line-height: 150%; margin: 0;'>Thank you for your purchase! Your order has been successfully placed.</p>
    //            </div>
    //        </div>
    //    </div>

    //    <!-- Content -->
    //    <div style='padding: 20px 30px; border-bottom: 1px solid #FFF;'>
    //        <div style='margin-bottom: 16px;'>
    //            <h2 style='color: #000; font-size: 16px; font-weight: 700; line-height: 150%; margin-bottom: 8px;'>Hello {WebUtility.HtmlEncode(shopperName)},</h2>
    //            <p style='color: #000; font-size: 16px; font-weight: 400; line-height: 150%; margin: 0;'>Your order has been confirmed and will be shipping soon.</p>
    //        </div>

    //        <!-- Order Info -->
    //        <div style='background: #FFF; border: 1px solid rgba(139, 139, 139, 0.08); border-radius: 4px; padding: 24px; margin-bottom: 16px; display: flex; flex-wrap: wrap; gap: 24px;'>
    //            <div style='flex: 1; min-width: 120px;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px; display: block; margin-bottom: 4px; white-space: nowrap;'>Order ID</span>
    //                <span style='color: #585858; font-size: 16px; font-weight: 600; line-height: 20px;'>{WebUtility.HtmlEncode(orderdto.OrderId.ToString())}</span>
    //            </div>
    //            <div style='flex: 1; min-width: 120px;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px; display: block; margin-bottom: 4px; white-space: nowrap;'>Order Date</span>
    //                <span style='color: #585858; font-size: 16px; font-weight: 600; line-height: 20px;'>{orderdto.OrderDate:MMMM dd, yyyy}</span>
    //            </div>
    //            <div style='flex: 1; min-width: 120px;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px; display: block; margin-bottom: 4px; white-space: nowrap;'>Total Amount</span>
    //                <span style='color: #585858; font-size: 16px; font-weight: 600; line-height: 20px;'>₹{orderdto.TotalAmount:F2}</span>
    //            </div>
    //        </div>

    //        <!-- Stores -->
    //        <div style='border: 1px solid rgba(139, 139, 139, 0.08); border-radius: 4px; overflow: hidden;'>
    //            {storesBuilder}
    //        </div>
    //    </div>

    //    <!-- Order Summary -->
    //    <div style='padding: 24px 30px;'>
    //        <h2 style='color: #000; font-size: 18px; font-weight: 500; margin-bottom: 16px;'>Order Summary</h2>
    //        <div style='border: 1px solid #E5E7EB; border-radius: 12px; padding: 20px; display: flex; flex-direction: column; gap: 16px;'>
    //            <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>Order ID</span>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>{WebUtility.HtmlEncode(orderdto.OrderId.ToString())}</span>
    //            </div>
    //            <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>Subtotal</span>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>₹{subtotal:F2}</span>
    //            </div>
    //            <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>Shipping</span>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>₹{shipping:F2}</span>
    //            </div>
    //            <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>Tax</span>
    //                <span style='color: #585858; font-size: 14px; font-weight: 500; line-height: 20px;'>₹{tax:F2}</span>
    //            </div>
    //            <div style='height: 1px; background: #D9D9D9;'></div>
    //            <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
    //                <span style='color: #000; font-size: 16px; font-weight: 600;'>Total Amount</span>
    //                <span style='color: #000; font-size: 16px; font-weight: 600;'>₹{orderdto.TotalAmount:F2}</span>
    //            </div>
    //        </div>
    //    </div>

    //    <!-- Payment -->
    //    <div style='padding: 24px 30px; border-top: 1px solid #F1F1F3;'>
    //        <h2 style='color: #000; font-size: 18px; font-weight: 500; margin-bottom: 12px;'>Payment Method</h2>
    //        <p style='color: #585858; font-size: 14px; font-weight: 500; margin-bottom: 4px;'>{WebUtility.HtmlEncode(orderdto.PaymentMethod)}</p>
    //        <p style='color: #16A34A; font-size: 14px; font-weight: 500;'>Payment successful</p>
    //    </div>

    //    <!-- Shipping -->
    //    <div style='padding: 24px 30px; border-top: 1px solid #F1F1F3;'>
    //        <h2 style='color: #000; font-size: 18px; font-weight: 500; margin-bottom: 12px;'>Shipping method</h2>
    //        <p style='color: #585858; font-size: 14px; font-weight: 500;'>P2P Delivery</p>
    //    </div>

    //    <!-- Delivery Address -->
    //    <div style='padding: 24px 30px; border-top: 1px solid #F1F1F3;'>
    //        <h2 style='color: #000; font-size: 18px; font-weight: 500; margin-bottom: 12px;'>Delivery Address</h2>
    //        <p style='color: #000; font-size: 14px; font-weight: 600; margin-bottom: 4px;'>{WebUtility.HtmlEncode(orderdto.ShopperName)}</p>
    //        <p style='color: #585858; font-size: 14px; font-weight: 500; line-height: 150%; margin-bottom: 8px;'>{WebUtility.HtmlEncode(orderdto.DeliveryAddress)}</p>
    //        <div style='display: flex; align-items: center; gap: 8px; color: #585858; font-size: 14px; font-weight: 500;'>
    //            <div style='width: 28px; height: 28px; border-radius: 100px; background: #F5F5F5; display: flex; align-items: center; justify-content: center; flex-shrink: 0;'>
    //                <span style='font-size: 12px;'>📞</span>
    //            </div>
    //            <span>{WebUtility.HtmlEncode(orderdto.ShopperPhone)}</span>
    //        </div>
    //    </div>

    //    <!-- CTA Button -->
    //    <div style='padding: 24px 30px; display: flex; justify-content: center;'>
    //        <a href='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/orders/{WebUtility.HtmlEncode(orderdto.OrderId.ToString())}' style='background: #004481; color: white; border: 1px solid #004481; border-radius: 8px; padding: 16px 32px; font-size: 16px; font-weight: 400; cursor: pointer; text-decoration: none; display: inline-block; text-align: center; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, sans-serif; transition: background 0.2s;'>View Order</a>
    //    </div>

    //    <!-- Footer -->
    //    <div style='padding: 20px 30px; background: rgba(139, 139, 139, 0.08);'>
    //        <div style='display: flex; flex-wrap: wrap; justify-content: center; gap: 24px; margin-bottom: 8px;'>
    //            <a href='#' style='color: #004481; font-size: 16px; font-weight: 400; text-decoration: none;'>Continue Shopping</a>
    //            <a href='#' style='color: #004481; font-size: 16px; font-weight: 400; text-decoration: none;'>My Account</a>
    //            <a href='#' style='color: #004481; font-size: 16px; font-weight: 400; text-decoration: none;'>Help Center</a>
    //        </div>
    //        <p style='color: #585858; font-size: 12px; font-weight: 400; text-align: center; line-height: 150%; margin: 8px 0;'>You're receiving this email because you placed an order with us.</p>
    //        <p style='color: #585858; font-size: 12px; font-weight: 400; text-align: center; line-height: 150%; margin: 8px 0;'>© 2026 itismytown. All rights reserved.</p>
    //        <a href='#' style='color: #004481; font-size: 12px; font-weight: 400; text-align: center; text-decoration: underline; display: block; margin-top: 8px;'>Unsubscribe from marketing emails</a>
    //    </div>

    //</div>

    //</body>
    //</html>";
    //}

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

        var productsBuilder = new StringBuilder();

        foreach (var item in storedto.Items)
        {
            string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                ? "https://via.placeholder.com/80x80?text=No+Image"
                : item.ImageUrl;

            productsBuilder.Append($@"
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""border:1px solid #E5E7EB;border-radius:12px;margin-bottom:12px;"">
  <tr>
    <td style=""padding:12px;"">

      <table width=""100%"">
        <tr>
          <td width=""80"">
            <img src=""{imageBaseUrl}/{Uri.EscapeDataString(imageSrc)}""
                 width=""80"" height=""80""
                 style=""border-radius:8px;object-fit:cover;"" />
          </td>
          <td>
            <b>{WebUtility.HtmlEncode(item.ProductName)}</b><br/>
            Qty: {item.Quantity}
          </td>
        </tr>
      </table>

    </td>
  </tr>
</table>");
        }

        return $@"
<html>
<body style='margin:0;padding:0;background:#FAFBFC;font-family:Segoe UI;'>

<table width='100%' bgcolor='#FAFBFC'>
<tr>
<td align='center'>

<table width='600' style='background:#fff;'>

<!-- HEADER -->
<tr>
<td align='center' style='padding:20px;background:#fff;'>
<img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' height='50'/>
</td>
</tr>

<!-- HERO -->
<tr>
<td style='padding:30px;text-align:center;background:linear-gradient(180deg,#155E75,#ffffff);'>
<h2>📦 Delivery Assignment Confirmed</h2>
<p>Please deliver this order to customer</p>
</td>
</tr>

<!-- CONTENT -->
<tr>
<td style='padding:20px;'>

<p>Hello <b>{WebUtility.HtmlEncode(courierName)}</b>,</p>

<p>
You have been assigned a delivery from 
<b>{WebUtility.HtmlEncode(storedto.StoreName)}</b>
</p>

<p>
<b>Order ID:</b> ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}<br/>
<b>Store Order ID:</b> {storedto.StoreOrderId}
</p>

<h3>Products</h3>
{productsBuilder}

<h3>Customer Details</h3>
<p>
{orderdto.ShopperName}<br/>
{orderdto.ShopperPhone}<br/>
{orderdto.DeliveryAddress}
</p>

<p><b>Deliver by:</b> {storedto.EstimatedDeliveryDate:dd MMM yyyy}</p>

</td>
</tr>

<!-- FOOTER -->
<tr>
<td style='background:#f1f1f1;text-align:center;padding:10px;font-size:12px;'>
© 2026 ItIsMyTown
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

    public async Task SendBranchLoginEmailAsync(string email, string password)
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
                    Subject = "Courier Branch Login Credentials - MyTown",
                    Body = $@"
<div style='font-family: Arial, sans-serif; background-color: #ffffff; padding: 40px; text-align: center;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 30px; border-radius: 10px; 
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2); border: 2px solid #004481;'>

        <!-- MyTown Logo -->
        <img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' 
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

        var productsBuilder = new StringBuilder();

        foreach (var item in storedto.Items)
        {
            string imageSrc = string.IsNullOrEmpty(item.ImageUrl)
                ? "https://via.placeholder.com/80x80?text=No+Image"
                : $"{imageBaseUrl}/{Uri.EscapeDataString(item.ImageUrl)}";

            productsBuilder.Append($@"
<tr>
<td style='padding:10px 0;border-bottom:1px solid #eee;'>
    <table width='100%'>
        <tr>
            <td width='80'>
                <img src='{imageSrc}' width='70' height='70'
                     style='border-radius:6px;object-fit:cover;' />
            </td>
            <td>
                <div style='font-weight:600;color:#333;'>
                    {WebUtility.HtmlEncode(item.ProductName)}
                </div>
                <div style='font-size:13px;color:#777;'>
                    Qty: {item.Quantity}
                </div>
            </td>
            <td align='right' style='font-weight:600;color:#333;'>
                ₹{item.ItemTotal:F2}
            </td>
        </tr>
    </table>
</td>
</tr>");
        }

        return $@"
<div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 30px;'>

<div style='max-width: 650px; margin: auto; background: white; padding: 25px; border-radius: 10px; 
            box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.15); border-top: 5px solid #004481;'>

    <!-- Logo -->
    <div style='text-align:center; margin-bottom:20px;'>
        <img src='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/images/mainlogoblue.png' 
             width='120' />
    </div>

    <!-- Title -->
    <h2 style='color:#004481; text-align:center;'>Transport Order Assigned</h2>

    <p style='text-align:center; color:#555; font-size:14px;'>
        You have been assigned a new delivery. Please check the details below.
    </p>

    <hr style='margin:20px 0;' />

    <!-- Greeting -->
    <p><strong>Hello {WebUtility.HtmlEncode(transporterName)}</strong>,</p>

    <p>
        You have been assigned a delivery from 
        <strong>{WebUtility.HtmlEncode(storedto.StoreName)}</strong>.
    </p>

    <!-- Order Info -->
    <div style='background:#f9fafb;padding:15px;border-radius:8px;margin-bottom:20px;'>
        <p><strong>Order ID:</strong> ITMT-{orderdto.OrderDate.Year}-{orderdto.OrderId:D6}</p>
        <p><strong>Order Date:</strong> {orderdto.OrderDate:dd MMM yyyy}</p>
        <p><strong>Store Order ID:</strong> {storedto.StoreOrderId}</p>
    </div>

    <!-- Products -->
    <h3 style='color:#004481;'>Products</h3>

    <table width='100%' cellpadding='0' cellspacing='0'>
        {productsBuilder}
    </table>

    <!-- Delivery Info -->
    <div style='background:#ecfdf5;padding:15px;border-radius:8px;margin-top:20px;'>
        <p style='margin:0;color:#065f46;font-weight:600;'>
            Deliver by: {storedto.EstimatedDeliveryDate:dd MMM yyyy}
        </p>
    </div>

    <!-- Customer Info -->
    <h3 style='color:#004481;margin-top:25px;'>Customer Details</h3>

    <div style='background:#f9fafb;padding:15px;border-radius:8px;'>
        <p><strong>{WebUtility.HtmlEncode(orderdto.ShopperName)}</strong></p>
        <p>{WebUtility.HtmlEncode(orderdto.ShopperPhone)}</p>
        <p>{WebUtility.HtmlEncode(orderdto.ShopperEmail)}</p>
        <p style='margin-top:10px;'>
            {WebUtility.HtmlEncode(orderdto.DeliveryAddress)}
        </p>
    </div>

    <!-- Button -->
    <div style='text-align:center;margin-top:25px;'>
        <a href='https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/orders/{orderdto.OrderId}'
           style='background:#004481;color:white;padding:12px 25px;
                  text-decoration:none;border-radius:6px;display:inline-block;'>
            View Order
        </a>
    </div>

    <hr style='margin:25px 0;' />

    <!-- Footer -->
    <p style='text-align:center;font-size:12px;color:#777;'>
        You're receiving this email because you're a transporter on MyTown.
    </p>

    <p style='text-align:center;font-size:11px;color:#aaa;'>
        © 2026 MyTown. All rights reserved.
    </p>

</div>
</div>";
    }

}
