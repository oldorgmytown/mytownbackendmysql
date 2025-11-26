using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using Stripe;
using Stripe.Climate;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static mytown.Models.busprofilepreview;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mytown.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<(int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();
    }
    public class UserRepository
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        public UserRepository(AppDbContext context, IConfiguration config, ITokenService tokenService)
        {
            _context = context;
            _config = config;
            _tokenService = tokenService;
        }





        //public async Task<object> LoginAsync(string email, string password)
        //{
        //    // 🔹 Business login
        //    var businessUser = await _context.BusinessRegisters.FirstOrDefaultAsync(r => r.BusEmail == email);
        //    if (businessUser != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, businessUser.Password))
        //        {
        //            var businessProfile = await _context.BusinessProfiles
        //                .Where(bp => bp.BusRegId == businessUser.BusRegId)
        //                .Select(bp => new
        //                {
        //                    bp.BusinessProfileId,
        //                    bp.BusinessName,
        //                    bp.BusinessLocation,
        //                    bp.BusinessAbout,
        //                    bp.BannerPath,
        //                    bp.LogoPath,
        //                    bp.ProfileStatus,
        //                    // bp.bus_time,
        //                    bp.BusCatId,
        //                    bp.BusServId,
        //                    //bp.Businessservice_name,
        //                    //bp.Businesscategory_name,
        //                    bp.ApprovedDate

        //                })
        //                .FirstOrDefaultAsync();

        //            return new
        //            {
        //                userType = "Business",
        //                user = new BusinessRegisterDto
        //                {
        //                    BusRegId = businessUser.BusRegId,
        //                    BusinessUsername = businessUser.BusinessUsername,
        //                    Businessname = businessUser.BusinessName,
        //                    LicenseType = businessUser.LicenseType,
        //                    Gstin = businessUser.Gstin,
        //                    BusservId = businessUser.BusServId,
        //                    BuscatId = businessUser.BusCatId,
        //                    Town = businessUser.Town,
        //                    BusMobileNo = businessUser.BusMobileNo,
        //                    BusEmail = businessUser.BusEmail,
        //                    Address1 = businessUser.Address1,
        //                    Address2 = businessUser.Address2,
        //                    businessCity = businessUser.BusinessCity,
        //                    businessState = businessUser.BusinessState,
        //                    businessCountry = businessUser.BusinessCountry,
        //                    postalCode = businessUser.PostalCode,
        //                    isEmailVerified = businessUser.IsEmailVerified,
        //                    BusinessRegDate = businessUser.BusinessRegDate,
        //                    ProfileStatus = businessProfile?.ProfileStatus ?? "Incomplete"
        //                },
        //                businessProfile = businessProfile // will be null if no profile exists
        //            };
        //        }

        //        return null; // invalid password
        //    }


        //    // 🔹 Shopper login
        //    var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
        //    if (shopper != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, shopper.Password))
        //        {
        //            return new
        //            {
        //                userType = "Shopper",
        //                shopper = new ShopperRegisterDto
        //                {
        //                    ShopperRegId = shopper.ShopperRegId,
        //                    Username = shopper.Username,
        //                    Email = shopper.Email,
        //                    IsEmailVerified = shopper.IsEmailVerified,
        //                    Address = shopper.Address,
        //                    Town = shopper.Town,
        //                    City = shopper.City,
        //                    State = shopper.State,
        //                    Country = shopper.Country,
        //                    PostalCode = shopper.PostalCode,
        //                    PhoneNumber = shopper.PhoneNumber,
        //                    PhotoName = shopper.PhotoName,
        //                    Status = shopper.Status,
        //                    ShopperRegDate = shopper.ShopperRegDate
        //                }
        //            };
        //        }
        //        return null;
        //    }

        //    // 🔹 Courier login
        //    var courier = await _context.CourierService.FirstOrDefaultAsync(c => c.CourierEmail == email);
        //    if (courier != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, courier.Password))
        //        {
        //            return new
        //            {
        //                userType = "Courier",
        //                courier = new CourierServiceDto
        //                {
        //                    CourierServiceName = courier.CourierServiceName,
        //                    CourierContactName = courier.CourierContactName,
        //                    CourierPhone = courier.CourierPhone,
        //                    CourierEmail = courier.CourierEmail,
        //                    IsLocal = courier.IsLocal,
        //                    IsState = courier.IsState,
        //                    IsNational = courier.IsNational,
        //                    IsInternational = courier.IsInternational
        //                }
        //            };
        //        }
        //        return null;
        //    }

        //    return null; // email not found
        //}

        public async Task<object> LoginAsync(string email, string password)
        {
            // ---------------- ADMIN LOGIN ----------------
            if (email == "admin@itismytown.com")
            {
                // You can store admin password in appsettings.json later
                if (password == "admin123")
                {
                    // Kill old active sessions
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == 1 && s.UserType == "Admin" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    // Create new admin session
                    var newSession = new UserSession
                    {
                        UserId = 1,   // static ID for admin
                        UserType = "Admin",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    // Generate token for admin
                    var token = _tokenService.GenerateToken(
                        1,
                        "admin@itismytown.com",
                        "Admin",
                        newSession.SessionGuid
                    );

                    return new
                    {
                        userType = "Admin",
                        token,
                        sessionId = newSession.SessionGuid,
                        admin = new
                        {
                            AdminId = 1,
                            Email = "admin@itismytown.com"
                            
                        }
                    };
                }
                return null;
            }

            // ---------------- BUSINESS LOGIN ----------------
            var businessUser = await _context.BusinessRegisters.FirstOrDefaultAsync(r => r.BusEmail == email);
            if (businessUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, businessUser.Password))
                {
                    //  Kill old session
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == businessUser.BusRegId && s.UserType == "Business" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    //  Create new session
                    var newSession = new UserSession
                    {
                        UserId = businessUser.BusRegId,
                        UserType = "Business",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    //  Get profile
                    var businessProfile = await _context.BusinessProfiles
                        .Where(bp => bp.BusRegId == businessUser.BusRegId)
                        .Select(bp => new
                        {
                            bp.BusinessProfileId,
                            bp.BusinessName,
                            bp.BusinessLocation,
                            bp.BusinessAbout,
                            bp.BannerPath,
                            bp.LogoPath,
                            bp.ProfileStatus,
                            bp.BusCatId,
                            bp.BusServId,
                            bp.ApprovedDate
                        })
                        .FirstOrDefaultAsync();

                    // Token
                    var token = _tokenService.GenerateToken(businessUser.BusRegId, businessUser.BusEmail, "Business", newSession.SessionGuid);

                    return new
                    {
                        userType = "Business",
                        token,
                        sessionId = newSession.SessionGuid,
                        user = new BusinessRegisterDto
                        {
                            BusRegId = businessUser.BusRegId,
                            BusinessUsername = businessUser.BusinessUsername,
                            Businessname = businessUser.BusinessName,
                            LicenseType = businessUser.LicenseType,
                            Gstin = businessUser.Gstin,
                            BusservId = businessUser.BusServId,
                            BuscatId = businessUser.BusCatId,
                            Town = businessUser.Town,
                            BusMobileNo = businessUser.BusMobileNo,
                            BusEmail = businessUser.BusEmail,
                            Address1 = businessUser.Address1,
                            Address2 = businessUser.Address2,
                            businessCity = businessUser.BusinessCity,
                            businessState = businessUser.BusinessState,
                            businessCountry = businessUser.BusinessCountry,
                            postalCode = businessUser.PostalCode,
                            isEmailVerified = businessUser.IsEmailVerified,
                            BusinessRegDate = businessUser.BusinessRegDate,
                            ProfileStatus = businessProfile?.ProfileStatus ?? "Incomplete"
                        },
                        businessProfile
                    };
                }
                return null;
            }

            // ---------------- SHOPPER LOGIN ----------------
            var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
            if (shopper != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, shopper.Password))
                {
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == shopper.ShopperRegId && s.UserType == "Shopper" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    var newSession = new UserSession
                    {
                        UserId = shopper.ShopperRegId,
                        UserType = "Shopper",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    var token = _tokenService.GenerateToken(shopper.ShopperRegId, shopper.Email, "Shopper", newSession.SessionGuid);

                    return new
                    {
                        userType = "Shopper",
                        token,
                        sessionId = newSession.SessionGuid,
                        shopper = new ShopperRegisterDto
                        {
                            ShopperRegId = shopper.ShopperRegId,
                            Username = shopper.Username,
                            Email = shopper.Email,
                            IsEmailVerified = shopper.IsEmailVerified,
                            Address = shopper.Address,
                            Town = shopper.Town,
                            City = shopper.City,
                            State = shopper.State,
                            Country = shopper.Country,
                            PostalCode = shopper.PostalCode,
                            PhoneNumber = shopper.PhoneNumber,
                            PhotoName = shopper.PhotoName,
                            Status = shopper.Status,
                            ShopperRegDate = shopper.ShopperRegDate
                        }
                    };
                }
                return null;
            }

            // ---------------- COURIER LOGIN ----------------
            var courier = await _context.CourierService.FirstOrDefaultAsync(c => c.CourierEmail == email);
            if (courier != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, courier.Password))
                {
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == courier.CourierId && s.UserType == "Courier" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    var newSession = new UserSession
                    {
                        UserId = courier.CourierId,
                        UserType = "Courier",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    var token = _tokenService.GenerateToken(courier.CourierId, courier.CourierEmail, "Courier", newSession.SessionGuid);

                    return new
                    {
                        userType = "Courier",
                        token,
                        sessionId = newSession.SessionGuid,
                        courier = new CourierServiceDto
                        {
                            CourierServiceName = courier.CourierServiceName,
                            CourierContactName = courier.CourierContactName,
                            CourierPhone = courier.CourierPhone,
                            CourierEmail = courier.CourierEmail,
                            IsLocal = courier.IsLocal,
                            IsState = courier.IsState,
                            IsNational = courier.IsNational,
                            IsInternational = courier.IsInternational
                        }
                    };
                }
                return null;
            }

            return null;
        }


        // Deactivate (mark inactive) all existing sessions for a user
        public async Task DeactivateOldSessionsAsync(int userId)
        {
            var oldSessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            if (oldSessions.Count > 0)
            {
                foreach (var s in oldSessions)
                    s.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        // Create a new session record
        public async Task<UserSession> CreateSessionAsync(int userId, string userType, string sessionGuid, DateTime? expiresAt = null, string? ip = null, string? deviceInfo = null)
        {
            var session = new UserSession
            {
                UserId = userId,
                UserType = userType,
                SessionGuid = sessionGuid,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IpAddress = ip,
                DeviceInfo = deviceInfo
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        // Check if a session GUID is active
        public async Task<bool> IsSessionActiveAsync(string sessionGuid)
        {
            return await _context.UserSessions.AnyAsync(s => s.SessionGuid == sessionGuid && s.IsActive);
        }



    }
}

