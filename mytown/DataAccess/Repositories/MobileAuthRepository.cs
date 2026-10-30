// mytown/DataAccess/Repositories/MobileAuthRepository.cs
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using MyTown.Models;
using System.Text.Json;

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

        // Everything needed to build the real entity later, stored in JsonPayload
        // until the OTP is verified.
        private class PendingSignupPayload
        {
            public string Role { get; set; } = "";
            public string ContactName { get; set; } = "";
            public string Email { get; set; } = "";
            public string HashedPassword { get; set; } = "";
            public string? MobileNo { get; set; }
            public string? Address { get; set; }
            public string? Town { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Country { get; set; }
            public string? PostalCode { get; set; }
            public string? BusinessName { get; set; }
            public string? BusinessType { get; set; }
            public int? BusCatId { get; set; }
            public int? BusServId { get; set; }

            // Bank Account Details
            public string? AccountHolderName { get; set; }
            public string? BankName { get; set; }
            public string? AccountNumber { get; set; }
            public string? IFSCCode { get; set; }
        }

        public async Task<(bool success, string message)> SignupAsync(MobileSignupDto dto)
        {

            string role = dto.Role.ToLower();

            bool emailExists = await EmailExistsAsync(dto.Email, role);

            if (emailExists)
                return (false, "Email already registered.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            string otp = GenerateOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(5);

            var payload = new PendingSignupPayload
            {
                Role = role,
                ContactName = dto.ContactName,
                Email = dto.Email,
                HashedPassword = hashedPassword,
                MobileNo = dto.MobileNo,
                Address = dto.Address,
                Town = dto.Town,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                BusinessName = dto.BusinessName,
                BusinessType = dto.BusinessType,
                AccountHolderName = dto.AccountHolderName,
                BankName = dto.BankName,
                AccountNumber = dto.AccountNumber,
                IFSCCode = dto.IFSCCode
            };

            if (role == "business")
            {
                var businessType = dto.BusinessType?.ToLower();
                payload.BusCatId = businessType == "services" ? 0 : 1;
                payload.BusServId = businessType == "products" ? 0 : 1;
            }

            string json = JsonSerializer.Serialize(payload);

            switch (role)
            {
                case "shopper":
                    await UpsertPendingAsync(
                        () => _context.PendingVerifications.FirstOrDefaultAsync(x => x.Email == dto.Email),
                        existing => { existing.Token = otp; existing.ExpiryDate = expiry; existing.JsonPayload = json; },
                        () => _context.PendingVerifications.Add(new PendingVerification
                        {
                            Email = dto.Email,
                            Token = otp,
                            ExpiryDate = expiry,
                            JsonPayload = json
                        }));
                    break;

                case "business":
                    await UpsertPendingAsync(
                        () => _context.PendingBusinessVerifications.FirstOrDefaultAsync(x => x.Email == dto.Email),
                        existing => { existing.Token = otp; existing.ExpiryDate = expiry; existing.JsonPayload = json; },
                        () => _context.PendingBusinessVerifications.Add(new PendingBusinessVerification
                        {
                            Email = dto.Email,
                            Token = otp,
                            ExpiryDate = expiry,
                            JsonPayload = json
                        }));
                    break;

                case "sender":
                    await UpsertPendingAsync(
                        () => _context.PendingSenderVerifications.FirstOrDefaultAsync(x => x.Email == dto.Email),
                        existing => { existing.Token = otp; existing.ExpiryDate = expiry; existing.JsonPayload = json; },
                        () => _context.PendingSenderVerifications.Add(new PendingSenderVerification
                        {
                            Email = dto.Email,
                            Token = otp,
                            ExpiryDate = expiry,
                            JsonPayload = json
                        }));
                    break;

                case "transporter":
                    await UpsertPendingAsync(
                        () => _context.PendingTransporterVerifications.FirstOrDefaultAsync(x => x.Email == dto.Email),
                        existing => { existing.Token = otp; existing.ExpiryDate = expiry; existing.JsonPayload = json; },
                        () => _context.PendingTransporterVerifications.Add(new PendingTransporterVerification
                        {
                            Email = dto.Email,
                            Token = otp,
                            ExpiryDate = expiry,
                            JsonPayload = json
                        }));
                    break;

                case "courier":
                    await UpsertPendingAsync(
                        () => _context.PendingCourierVerifications.FirstOrDefaultAsync(x => x.Email == dto.Email),
                        existing => { existing.Token = otp; existing.ExpiryDate = expiry; existing.JsonPayload = json; },
                        () => _context.PendingCourierVerifications.Add(new PendingCourierVerification
                        {
                            Email = dto.Email,
                            Token = otp,
                            ExpiryDate = expiry,
                            JsonPayload = json
                        }));
                    break;

                default:
                    return (false, "Invalid role.");
            }

            await _context.SaveChangesAsync();
            await _emailService.SendOtpEmailAsync(dto.Email, dto.ContactName, otp);
            return (true, "Registration successful. OTP sent to your email.");
        }

        // Small helper so we don't duplicate the "update-if-exists-else-add" logic 5 times
        private async Task UpsertPendingAsync<T>(Func<Task<T?>> find, Action<T> update, Action add) where T : class
        {
            var existing = await find();
            if (existing != null)
                update(existing);
            else
                add();
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
                    if (existing == null)
                        return (false, "No pending signup found for this email.");
                    existing.Token = otp;
                    existing.ExpiryDate = expiry;
                    var shopperPayload = Deserialize(existing.JsonPayload);
                    if (shopperPayload != null) name = shopperPayload.ContactName;
                    break;

                case "business":
                    var existingB = await _context.PendingBusinessVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingB == null)
                        return (false, "No pending signup found for this email.");
                    existingB.Token = otp;
                    existingB.ExpiryDate = expiry;
                    var businessPayload = Deserialize(existingB.JsonPayload);
                    if (businessPayload != null) name = businessPayload.BusinessName ?? businessPayload.ContactName;
                    break;

                case "sender":
                    var existingS = await _context.PendingSenderVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingS == null)
                        return (false, "No pending signup found for this email.");
                    existingS.Token = otp;
                    existingS.ExpiryDate = expiry;
                    var senderPayload = Deserialize(existingS.JsonPayload);
                    if (senderPayload != null) name = senderPayload.ContactName;
                    break;

                case "transporter":
                    var existingT = await _context.PendingTransporterVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingT == null)
                        return (false, "No pending signup found for this email.");
                    existingT.Token = otp;
                    existingT.ExpiryDate = expiry;
                    var transporterPayload = Deserialize(existingT.JsonPayload);
                    if (transporterPayload != null) name = transporterPayload.ContactName;
                    break;

                case "courier":
                    var existingC = await _context.PendingCourierVerifications
                        .FirstOrDefaultAsync(x => x.Email == email);
                    if (existingC == null)
                        return (false, "No pending signup found for this email.");
                    existingC.Token = otp;
                    existingC.ExpiryDate = expiry;
                    var courierPayload = Deserialize(existingC.JsonPayload);
                    if (courierPayload != null) name = courierPayload.BusinessName ?? courierPayload.ContactName;
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
            switch (role.ToLower())
            {
                case "shopper":
                    {
                        var sv = await _context.PendingVerifications
                            .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                        if (sv == null || sv.ExpiryDate < DateTime.UtcNow)
                            return (false, "Invalid or expired OTP.");

                        var payload = Deserialize(sv.JsonPayload);
                        if (payload == null)
                            return (false, "Signup data could not be found. Please sign up again.");

                        var shopper = new ShopperRegister
                        {
                            Username = payload.ContactName,
                            Email = payload.Email,
                            Password = payload.HashedPassword,
                            PhoneNumber = payload.MobileNo,
                            Address = payload.Address ?? "",
                            Town = payload.Town ?? "",
                            City = payload.City ?? "",
                            State = payload.State ?? "",
                            Country = payload.Country ?? "",
                            IsEmailVerified = true
                        };
                        _context.ShopperRegisters.Add(shopper);

                        _context.PendingVerifications.Remove(sv);
                        await _context.SaveChangesAsync();
                        break;
                    }

                case "business":
                    {
                        var bv = await _context.PendingBusinessVerifications
                            .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                        if (bv == null || bv.ExpiryDate < DateTime.UtcNow)
                            return (false, "Invalid or expired OTP.");

                        var payload = Deserialize(bv.JsonPayload);
                        if (payload == null)
                            return (false, "Signup data could not be found. Please sign up again.");

                        var business = new BusinessRegister
                        {
                            BusinessName = payload.BusinessName ?? "",
                            BusinessUsername = payload.ContactName,
                            BusEmail = payload.Email,
                            Password = payload.HashedPassword,
                            BusMobileNo = payload.MobileNo,
                            Address1 = payload.Address ?? "",
                            Town = payload.Town ?? "",
                            BusinessCity = payload.City ?? "",
                            BusinessState = payload.State ?? "",
                            BusinessCountry = payload.Country ?? "",
                            PostalCode = payload.PostalCode,
                            IsEmailVerified = true,
                            LicenseType = "Pending",
                            Gstin = "",
                            BusServId = payload.BusServId ?? 1,
                            BusCatId = payload.BusCatId ?? 1
                        };
                        _context.BusinessRegisters.Add(business);
                        // Save now so EF populates business.BusRegId for the profile/account FK below
                        await _context.SaveChangesAsync();

                        var businessProfile = new BusinessProfile
                        {
                            BusRegId = business.BusRegId,
                            ProfileStatus = "Incomplete",
                            BusinessName = business.BusinessName,
                            BusinessLocation = $"{business.Town}, {business.BusinessCity}, {business.BusinessState}, {business.BusinessCountry}"
                        };
                        _context.BusinessProfiles.Add(businessProfile);

                        // Save bank account details (only if provided at signup)
                        if (!string.IsNullOrWhiteSpace(payload.AccountNumber))
                        {
                            var bankDetails = new BusinessAccountDetail
                            {
                                BusRegId = business.BusRegId,
                                AccountHolderName = payload.AccountHolderName,
                                BankName = payload.BankName,
                                AccountNumber = payload.AccountNumber,
                                IFSCCode = payload.IFSCCode,
                                CreatedDate = DateTime.UtcNow
                            };
                            _context.Set<BusinessAccountDetail>().Add(bankDetails);
                        }

                        _context.PendingBusinessVerifications.Remove(bv);
                        await _context.SaveChangesAsync();
                        break;
                    }

                case "sender":
                    {
                        var senv = await _context.PendingSenderVerifications
                            .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                        if (senv == null || senv.ExpiryDate < DateTime.UtcNow)
                            return (false, "Invalid or expired OTP.");

                        var payload = Deserialize(senv.JsonPayload);
                        if (payload == null)
                            return (false, "Signup data could not be found. Please sign up again.");

                        var sender = new SenderRegister
                        {
                            SenderName = payload.ContactName,
                            Email = payload.Email,
                            Password = payload.HashedPassword,
                            PhoneNumber = payload.MobileNo,
                            Address = payload.Address ?? "",
                            Town = payload.Town ?? "",
                            City = payload.City ?? "",
                            State = payload.State ?? "",
                            Country = payload.Country ?? "",
                            IsEmailVerified = true
                        };
                        _context.SenderRegisters.Add(sender);

                        _context.PendingSenderVerifications.Remove(senv);
                        await _context.SaveChangesAsync();
                        break;
                    }

                case "transporter":
                    {
                        var tv = await _context.PendingTransporterVerifications
                            .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                        if (tv == null || tv.ExpiryDate < DateTime.UtcNow)
                            return (false, "Invalid or expired OTP.");

                        var payload = Deserialize(tv.JsonPayload);
                        if (payload == null)
                            return (false, "Signup data could not be found. Please sign up again.");

                        var transporter = new TransporterRegister
                        {
                            TransporterName = payload.ContactName,
                            Email = payload.Email,
                            Password = payload.HashedPassword,
                            PhoneNumber = payload.MobileNo,
                            Address = payload.Address ?? "",
                            Town = payload.Town ?? "",
                            City = payload.City ?? "",
                            State = payload.State ?? "",
                            Country = payload.Country ?? "",
                            IsEmailVerified = true
                        };
                        _context.TransporterRegisters.Add(transporter);
                        // Save now so EF populates transporter.TransporterRegId for the account FK below
                        await _context.SaveChangesAsync();

                        // Save bank account details (only if provided at signup)
                        if (!string.IsNullOrWhiteSpace(payload.AccountNumber))
                        {
                            var accountDetails = new TransporterAccountDetail
                            {
                                TransporterRegId = transporter.TransporterRegId,
                                AccountHolderName = payload.AccountHolderName,
                                BankName = payload.BankName,
                                AccountNumber = payload.AccountNumber,
                                IFSCCode = payload.IFSCCode,
                                IsTermsAccepted = true,
                                CreatedDate = DateTime.UtcNow
                            };
                            _context.Set<TransporterAccountDetail>().Add(accountDetails);
                        }

                        _context.PendingTransporterVerifications.Remove(tv);
                        await _context.SaveChangesAsync();
                        break;
                    }

                case "courier":
                    {
                        var cv = await _context.PendingCourierVerifications
                            .FirstOrDefaultAsync(x => x.Email == email && x.Token == otp);
                        if (cv == null || cv.ExpiryDate < DateTime.UtcNow)
                            return (false, "Invalid or expired OTP.");

                        var payload = Deserialize(cv.JsonPayload);
                        if (payload == null)
                            return (false, "Signup data could not be found. Please sign up again.");

                        var courier = new CourierService
                        {
                            CourierServiceName = payload.BusinessName ?? payload.ContactName,
                            CourierWebsiteName = "",
                            CourierEmail = payload.Email,
                            Password = payload.HashedPassword,
                            CourierPhone = payload.MobileNo,
                            Address = payload.Address ?? "",
                            Town = payload.Town ?? "",
                            City = payload.City ?? "",
                            State = payload.State ?? "",
                            Country = payload.Country ?? "",
                            PostalCode = payload.PostalCode ?? "",
                            IsEmailVerified = true
                        };
                        _context.CourierService.Add(courier);
                        // Save now so EF populates courier.CourierId for the account FK below
                        await _context.SaveChangesAsync();

                        // Save bank account details (only if provided at signup)
                        if (!string.IsNullOrWhiteSpace(payload.AccountNumber))
                        {
                            var bank = new CourierAccountDetail
                            {
                                CourierId = courier.CourierId,
                                AccountHolderName = payload.AccountHolderName,
                                BankName = payload.BankName,
                                AccountNumber = payload.AccountNumber,
                                IFSCCode = payload.IFSCCode,
                                CreatedDate = DateTime.UtcNow
                            };
                            _context.Set<CourierAccountDetail>().Add(bank);
                        }

                        _context.PendingCourierVerifications.Remove(cv);
                        await _context.SaveChangesAsync();
                        break;
                    }

                default:
                    return (false, "Invalid role.");
            }

            return (true, "Email verified successfully.");
        }

        private PendingSignupPayload? Deserialize(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            try
            {
                return JsonSerializer.Deserialize<PendingSignupPayload>(json);
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> EmailExistsAsync(string email, string role)
        {
            return role.ToLower() switch
            {
                "shopper" => await _context.ShopperRegisters
                    .AnyAsync(x => x.Email == email),

                "business" => await _context.BusinessRegisters
                    .AnyAsync(x => x.BusEmail == email),

                "sender" => await _context.SenderRegisters
                    .AnyAsync(x => x.Email == email),

                "transporter" => await _context.TransporterRegisters
                    .AnyAsync(x => x.Email == email),

                "courier" => await _context.CourierService
                    .AnyAsync(x => x.CourierEmail == email),

                _ => false
            };
        }
    }
}