using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using Stripe;
using Stripe.Climate;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static mytown.Models.busprofilepreview;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;
using mytown.DataAccess.Interfaces;

namespace mytown.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
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
                        user = new
                        {
                            AdminId = 1,
                            Email = "admin@itismytown.com",
                            Name = "Admin"
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
                        courierDto = new CourierServiceDto
                        {
                            CourierServiceName = courier.CourierServiceName,
                            CourierWebsiteName = courier.CourierWebsiteName,
                            CourierEmail = courier.CourierEmail,
                            CourierPhone = courier.CourierPhone,

                            // 📍 Main Office Location
                            Address = courier.Address,
                            Town = courier.Town,
                            City = courier.City,
                            State = courier.State,
                            Country = courier.Country,
                            PostalCode = courier.PostalCode,

                            // 🚚 Service Coverage
                            IsCity = courier.IsCity,
                            IsState = courier.IsState,

                            // 🔐 Auth (only during registration)
                            Password = courier.Password
                        }
                           // ConfirmPassword = courier.ConfirmPassword
                        };

                }
                return null;
            }

            return null;
        }

        public async Task<object> LoginAsyncwithRole(string email, string password, string role)
        {
            // ---------------- ADMIN LOGIN ----------------
            if (role == "Admin" && email == "admin@itismytown.com")
            {
                if (password == "admin123")
                {
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == 1 && s.UserType == "Admin" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    var newSession = new UserSession
                    {
                        UserId = 1,
                        UserType = "Admin",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

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
                        user = new
                        {
                            AdminId = 1,
                            Email = "admin@itismytown.com",
                            Name = "Admin"
                        }
                    };
                }
                return null;
            }

            // ---------------- BUSINESS LOGIN ----------------
            if (role == "Business")
            {
                var businessUser = await _context.BusinessRegisters
                    .FirstOrDefaultAsync(r => r.BusEmail == email);

                if (businessUser != null && BCrypt.Net.BCrypt.Verify(password, businessUser.Password))
                {
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == businessUser.BusRegId && s.UserType == "Business" && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

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

                    var token = _tokenService.GenerateToken(
                        businessUser.BusRegId,
                        businessUser.BusEmail,
                        "Business",
                        newSession.SessionGuid
                    );

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
            if (role == "Shopper")
            {
                var shopper = await _context.ShopperRegisters
                    .FirstOrDefaultAsync(s => s.Email == email);

                if (shopper != null && BCrypt.Net.BCrypt.Verify(password, shopper.Password))
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

                    var token = _tokenService.GenerateToken(
                        shopper.ShopperRegId,
                        shopper.Email,
                        "Shopper",
                        newSession.SessionGuid
                    );

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
            if (role == "CourierHead")
            {
                var courier = await _context.CourierService
                    .FirstOrDefaultAsync(c => c.CourierEmail.ToLower() == email.ToLower());

                if (courier != null)
                {
                    bool isValidPassword = false;

                    if (!string.IsNullOrEmpty(courier.Password) && courier.Password.StartsWith("$2"))
                    {
                        // BCrypt hashed password
                        isValidPassword = BCrypt.Net.BCrypt.Verify(password, courier.Password);
                    }
                    else
                    {
                        // Plain text password
                        isValidPassword = courier.Password == password;
                    }

                    if (isValidPassword)
                    {
                        var oldSession = await _context.UserSessions
                            .Where(s => s.UserId == courier.CourierId && s.UserType == "CourierHead" && s.IsActive)
                            .FirstOrDefaultAsync();

                        if (oldSession != null)
                        {
                            oldSession.IsActive = false;
                            _context.UserSessions.Update(oldSession);
                        }

                        var newSession = new UserSession
                        {
                            UserId = courier.CourierId,
                            UserType = "CourierHead",
                            SessionGuid = Guid.NewGuid().ToString(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.UserSessions.Add(newSession);
                        await _context.SaveChangesAsync();

                        // Check if branches exist for this courier
                        var hasBranches = await _context.CourierBranches
                            .AnyAsync(b => b.CourierId == courier.CourierId && b.IsActive);

                        var token = _tokenService.GenerateToken(
                            courier.CourierId,
                            courier.CourierEmail,
                            "CourierHead",
                            newSession.SessionGuid
                        );

                        return new
                        {
                            userType = "CourierHead",
                            token,
                            sessionId = newSession.SessionGuid,
                            hasBranches,
                            courier = new CourierServiceDto
                            {
                                CourierId = courier.CourierId,
                                CourierServiceName = courier.CourierServiceName,
                                CourierWebsiteName = courier.CourierWebsiteName,
                                CourierEmail = courier.CourierEmail,
                                CourierPhone = courier.CourierPhone,
                                Address = courier.Address,
                                Town = courier.Town,
                                City = courier.City,
                                State = courier.State,
                                Country = courier.Country,
                                PostalCode = courier.PostalCode,
                                IsCity = courier.IsCity,
                                IsState = courier.IsState
                            }
                        };
                    }
                }

                return null;
            }

            if (role == "CourierBranch")
            {
                var branch = await _context.CourierBranches
                    .FirstOrDefaultAsync(b => b.BranchEmailId == email && b.IsActive);

                if (branch != null &&
    (password == "Branch@123" ||
    (!string.IsNullOrEmpty(branch.PasswordHash) &&
     BCrypt.Net.BCrypt.Verify(password, branch.PasswordHash))))
                {
                    var oldSession = await _context.UserSessions
                        .FirstOrDefaultAsync(s =>
                            s.UserId == branch.BranchId &&
                            s.UserType == "CourierBranch" &&
                            s.IsActive);

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    var newSession = new UserSession
                    {
                        UserId = branch.BranchId,
                        UserType = "CourierBranch",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    var token = _tokenService.GenerateToken(
                        branch.BranchId,
                        branch.BranchEmailId,
                        "CourierBranch",
                        newSession.SessionGuid
                    );

                    return new
                    {
                        userType = "CourierBranch",
                        token,
                        sessionId = newSession.SessionGuid,
                        courier = new CourierBranchLoginDto
                        {
                            BranchId = branch.BranchId,
                            CourierId = branch.CourierId,
                            CourierServiceName = branch.CourierServiceName,
                            BranchEmailId = branch.BranchEmailId,
                            BranchPhoneNumber = branch.BranchPhoneNumber,
                            Country = branch.Country,
                            State = branch.State,
                            City = branch.City,
                            Town = branch.Town
                        }
                    };
                }

                return null;
            }

            // ---------------- TRANSPORTER LOGIN ----------------
            if (role == "Transporter")
            {
                var transporter = await _context.TransporterRegisters
                    .FirstOrDefaultAsync(t => t.Email == email);

                if (transporter != null && BCrypt.Net.BCrypt.Verify(password, transporter.Password))
                {
                    // ❗ Optional but recommended: check email verified
                    if (!transporter.IsEmailVerified)
                    {
                        return new
                        {
                            error = "Please verify your email before logging in."
                        };
                    }

                    // ❗ Optional: check status
                    if (transporter.Status == "Blocked")
                    {
                        return new
                        {
                            error = "Your account is blocked. Contact support."
                        };
                    }

                    // ---------------- SESSION HANDLING ----------------
                    var oldSession = await _context.UserSessions
                        .Where(s => s.UserId == transporter.TransporterRegId
                                 && s.UserType == "Transporter"
                                 && s.IsActive)
                        .FirstOrDefaultAsync();

                    if (oldSession != null)
                    {
                        oldSession.IsActive = false;
                        _context.UserSessions.Update(oldSession);
                    }

                    var newSession = new UserSession
                    {
                        UserId = transporter.TransporterRegId,
                        UserType = "Transporter",
                        SessionGuid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserSessions.Add(newSession);
                    await _context.SaveChangesAsync();

                    // ---------------- TOKEN ----------------
                    var token = _tokenService.GenerateToken(
                        transporter.TransporterRegId,
                        transporter.Email,
                        "Transporter",
                        newSession.SessionGuid
                    );

                    // ---------------- RESPONSE ----------------
                    return new
                    {
                        userType = "Transporter",
                        token,
                        sessionId = newSession.SessionGuid,
                        transporter = new TransporterRegisterDto
                        {
                            TransporterId = transporter.TransporterRegId,
                            TransporterName = transporter.TransporterName,
                            Email = transporter.Email,
                            PhoneNumber = transporter.PhoneNumber,
                            Address = transporter.Address,
                            Town = transporter.Town,
                            City = transporter.City,
                            State = transporter.State,
                            Country = transporter.Country,
                            PostalCode = transporter.PostalCode,
                            Status = transporter.Status,
                           // IsEmailVerified = transporter.IsEmailVerified.ToString(), // ⚠️ since your DTO has string
                            TransporterRegDate = transporter.TransporeterRegDate
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

