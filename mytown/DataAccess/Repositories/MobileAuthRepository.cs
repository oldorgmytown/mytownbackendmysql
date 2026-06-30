using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.DataAccess.Repositories
{
    public class MobileAuthRepository : IMobileAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public MobileAuthRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private string GenerateOtp() =>
            new Random().Next(100000, 999999).ToString();

        public async Task<(bool success, string message)> SignupAsync(MobileSignupDto dto)
        {
            bool emailExists = await EmailExistsAsync(dto.Email);
            if (emailExists)
                return (false, "Email already registered.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            string otp = GenerateOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(5);

            switch (dto.Role.ToLower())
            {
                case "shopper":
                    var shopper = new ShopperRegister
                    {
                        Username = dto.ContactName,
                        Email = dto.Email,
                        Password = hashedPassword,
                        PhoneNumber = dto.MobileNo,
                        Address = dto.Address ?? "",
                        Town = dto.Town ?? "",
                        City = dto.City ?? "",
                        State = dto.State ?? "",
                        Country = dto.Country ?? "",
                        IsEmailVerified = false
                    };
                    _context.ShopperRegisters.Add(shopper);
                    await _context.SaveChangesAsync();

                    _context.PendingVerifications.Add(new PendingVerification
                    {
                        Email = dto.Email,
                        Token = otp,
                        ExpiryDate = expiry,
                        JsonPayload = ""
                    });
                    break;

                case "business":
                    int busCatId = dto.BusinessType?.ToLower() == "services" ? 2 : 1;

                    var business = new BusinessRegister
                    {
                        BusinessName = dto.BusinessName ?? "",
                        BusinessUsername = dto.ContactName,
                        BusEmail = dto.Email,
                        Password = hashedPassword,
                        BusMobileNo = dto.MobileNo,
                        Address1 = dto.Address ?? "",
                        Town = dto.Town ?? "",
                        BusinessCity = dto.City ?? "",
                        BusinessState = dto.State ?? "",
                        BusinessCountry = dto.Country ?? "",
                        PostalCode = dto.PostalCode,
                        IsEmailVerified = false,
                        LicenseType = "Pending",
                        BusServId = 0,
                        BusCatId = busCatId
                    };
                    _context.BusinessRegisters.Add(business);
                    await _context.SaveChangesAsync();

                    _context.PendingBusinessVerifications.Add(new PendingBusinessVerification
                    {
                        Email = dto.Email,
                        Token = otp,
                        ExpiryDate = expiry,
                        JsonPayload = ""
                    });
                    break;

                case "sender":
                    var sender = new SenderRegister
                    {
                        SenderName = dto.ContactName,
                        Email = dto.Email,
                        Password = hashedPassword,
                        PhoneNumber = dto.MobileNo,
                        Address = dto.Address ?? "",
                        Town = dto.Town ?? "",
                        City = dto.City ?? "",
                        State = dto.State ?? "",
                        Country = dto.Country ?? ""
                    };
                    _context.SenderRegisters.Add(sender);
                    await _context.SaveChangesAsync();

                    _context.PendingSenderVerifications.Add(new PendingSenderVerification
                    {
                        Email = dto.Email,
                        Token = otp,
                        ExpiryDate = expiry,
                        JsonPayload = ""
                    });
                    break;

                case "transporter":
                    var transporter = new TransporterRegister
                    {
                        TransporterName = dto.ContactName,
                        Email = dto.Email,
                        Password = hashedPassword,
                        PhoneNumber = dto.MobileNo,
                        Address = dto.Address ?? "",
                        Town = dto.Town ?? "",
                        City = dto.City ?? "",
                        State = dto.State ?? "",
                        Country = dto.Country ?? ""
                    };
                    _context.TransporterRegisters.Add(transporter);
                    await _context.SaveChangesAsync();

                    _context.PendingTransporterVerifications.Add(new PendingTransporterVerification
                    {
                        Email = dto.Email,
                        Token = otp,
                        ExpiryDate = expiry,
                        JsonPayload = ""
                    });
                    break;

                case "courier":
                    var courier = new CourierService
                    {
                        CourierServiceName = dto.BusinessName ?? dto.ContactName,
                        CourierWebsiteName = "",
                        CourierEmail = dto.Email,
                        Password = hashedPassword,
                        CourierPhone = dto.MobileNo,
                        Address = dto.Address ?? "",
                        Town = dto.Town ?? "",
                        City = dto.City ?? "",
                        State = dto.State ?? "",
                        Country = dto.Country ?? "",
                        PostalCode = dto.PostalCode ?? "",
                        IsEmailVerified = false
                    };
                    _context.CourierService.Add(courier);
                    await _context.SaveChangesAsync();

                    _context.PendingCourierVerifications.Add(new PendingCourierVerification
                    {
                        Email = dto.Email,
                        Token = otp,
                        ExpiryDate = expiry,
                        JsonPayload = ""
                    });
                    break;

                default:
                    return (false, "Invalid role.");
            }

            await _context.SaveChangesAsync();
            await _emailService.SendOtpEmailAsync(dto.Email, dto.ContactName, otp);
            return (true, "Registration successful. OTP sent to your email.");
        }

        public async Task<(bool success, string message)> SendOtpAsync(string email, string role)
        {
            string otp = GenerateOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(5);
            string name = email;

            switch (role.ToLower())
            {
                case "shopper":
                    var existing = await _context.PendingVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existing != null) { existing.Token = otp; existing.ExpiryDate = expiry; }
                    else _context.PendingVerifications.Add(new PendingVerification { Email = email, Token = otp, ExpiryDate = expiry });
                    var s = await _context.ShopperRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (s != null) name = s.Username;
                    break;

                case "business":
                    var existingB = await _context.PendingBusinessVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingB != null) { existingB.Token = otp; existingB.ExpiryDate = expiry; }
                    else _context.PendingBusinessVerifications.Add(new PendingBusinessVerification { Email = email, Token = otp, ExpiryDate = expiry });
                    var b = await _context.BusinessRegisters.FirstOrDefaultAsync(x => x.BusEmail == email);
                    if (b != null) name = b.BusinessName;
                    break;

                case "sender":
                    var existingS = await _context.PendingSenderVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingS != null) { existingS.Token = otp; existingS.ExpiryDate = expiry; }
                    else _context.PendingSenderVerifications.Add(new PendingSenderVerification { Email = email, Token = otp, ExpiryDate = expiry });
                    var sn = await _context.SenderRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (sn != null) name = sn.SenderName;
                    break;

                case "transporter":
                    var existingT = await _context.PendingTransporterVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingT != null) { existingT.Token = otp; existingT.ExpiryDate = expiry; }
                    else _context.PendingTransporterVerifications.Add(new PendingTransporterVerification { Email = email, Token = otp, ExpiryDate = expiry });
                    var t = await _context.TransporterRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (t != null) name = t.TransporterName;
                    break;

                case "courier":
                    var existingC = await _context.PendingCourierVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingC != null) { existingC.Token = otp; existingC.ExpiryDate = expiry; }
                    else _context.PendingCourierVerifications.Add(new PendingCourierVerification { Email = email, Token = otp, ExpiryDate = expiry });
                    var c = await _context.CourierService.FirstOrDefaultAsync(x => x.CourierEmail == email);
                    if (c != null) name = c.CourierServiceName;
                    break;

                default:
                    return (false, "Invalid role.");
            }

            await _context.SaveChangesAsync();
            await _emailService.SendOtpEmailAsync(email, name, otp);
            return (true, "OTP sent successfully.");
        }

        public async Task<(bool success, string message)> VerifyOtpAsync(string email, string otp, string role)
        {
            bool isValid = false;

            switch (role.ToLower())
            {
                case "shopper":
                    var sv = await _context.PendingVerifications
                        .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                    if (sv == null || sv.ExpiryDate < DateTime.UtcNow)
                        return (false, "Invalid or expired OTP.");
                    var sh = await _context.ShopperRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (sh != null) sh.IsEmailVerified = true;
                    _context.PendingVerifications.Remove(sv);
                    isValid = true;
                    break;

                case "business":
                    var bv = await _context.PendingBusinessVerifications
                        .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                    if (bv == null || bv.ExpiryDate < DateTime.UtcNow)
                        return (false, "Invalid or expired OTP.");
                    var bus = await _context.BusinessRegisters.FirstOrDefaultAsync(x => x.BusEmail == email);
                    if (bus != null) bus.IsEmailVerified = true;
                    _context.PendingBusinessVerifications.Remove(bv);
                    isValid = true;
                    break;

                case "sender":
                    var senv = await _context.PendingSenderVerifications
                        .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                    if (senv == null || senv.ExpiryDate < DateTime.UtcNow)
                        return (false, "Invalid or expired OTP.");
                    var sen = await _context.SenderRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (sen != null) sen.IsEmailVerified = true;
                    _context.PendingSenderVerifications.Remove(senv);
                    isValid = true;
                    break;

                case "transporter":
                    var tv = await _context.PendingTransporterVerifications
                        .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                    if (tv == null || tv.ExpiryDate < DateTime.UtcNow)
                        return (false, "Invalid or expired OTP.");
                    var tr = await _context.TransporterRegisters.FirstOrDefaultAsync(x => x.Email == email);
                    if (tr != null) tr.IsEmailVerified = true;
                    _context.PendingTransporterVerifications.Remove(tv);
                    isValid = true;
                    break;

                case "courier":
                    var cv = await _context.PendingCourierVerifications
                        .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                    if (cv == null || cv.ExpiryDate < DateTime.UtcNow)
                        return (false, "Invalid or expired OTP.");
                    var co = await _context.CourierService.FirstOrDefaultAsync(x => x.CourierEmail == email);
                    if (co != null) co.IsEmailVerified = true;
                    _context.PendingCourierVerifications.Remove(cv);
                    isValid = true;
                    break;

                default:
                    return (false, "Invalid role.");
            }

            if (isValid)
                await _context.SaveChangesAsync();

            return (true, "Email verified successfully.");
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.ShopperRegisters.AnyAsync(x => x.Email == email)
                || await _context.BusinessRegisters.AnyAsync(x => x.BusEmail == email)
                || await _context.SenderRegisters.AnyAsync(x => x.Email == email)
                || await _context.TransporterRegisters.AnyAsync(x => x.Email == email)
                || await _context.CourierService.AnyAsync(x => x.CourierEmail == email);
        }
    }
}