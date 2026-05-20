using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTOs;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;

namespace mytown.DataAccess.Repositories
{
    public class BusinessServiceRepository : IBusinessServiceRepository
    {
        private readonly AppDbContext _context;
        public BusinessServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<mytown.Models.BusinessService>> GetAllServicesAsync()
        {
            return await _context.BusinessServices.ToListAsync();
        }

        public async Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId)
        {
            return await _context.ServiceSubCategory
                .Where(x => x.BusServId == busServId)
                .ToListAsync();
        }

        public async Task<bool> AddOrUpdateServiceProfileAsync(CreateServiceProfileDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check existing profile
                var existingProfile = await _context.ServiceProfiles
                    .FirstOrDefaultAsync(x => x.BusRegId == dto.BusRegId);

                if (existingProfile != null)
                {
                    // UPDATE PROFILE
                    existingProfile.BusServId = dto.BusServId;
                    existingProfile.YearsOfExperience = dto.YearsOfExperience;
                    existingProfile.GovtIdDocument = dto.GovtIdDocument;
                    existingProfile.ProfessionalLicense = dto.ProfessionalLicense;
                    existingProfile.ServiceAvailableLocations = dto.ServiceAvailableLocations;
                    existingProfile.WorkingDays = dto.WorkingDays;
                    existingProfile.WorkingStartTime = dto.WorkingStartTime;
                    existingProfile.WorkingEndTime = dto.WorkingEndTime;
                    existingProfile.ServiceLogo = dto.ServiceLogo;
                    existingProfile.ServiceBanner = dto.ServiceBanner;

                    _context.ServiceProfiles.Update(existingProfile);
                }
                else
                {
                    // INSERT PROFILE
                    var profile = new ServiceProfile
                    {
                        BusRegId = dto.BusRegId,
                        BusServId = dto.BusServId,
                        YearsOfExperience = dto.YearsOfExperience,
                        GovtIdDocument = dto.GovtIdDocument,
                        ProfessionalLicense = dto.ProfessionalLicense,
                        ServiceAvailableLocations = dto.ServiceAvailableLocations,
                        WorkingDays = dto.WorkingDays,
                        WorkingStartTime = dto.WorkingStartTime,
                        WorkingEndTime = dto.WorkingEndTime,
                        ServiceLogo = dto.ServiceLogo,
                        ServiceBanner = dto.ServiceBanner,
                        CreatedDate = DateTime.Now
                    };

                    _context.ServiceProfiles.Add(profile);
                }

                await _context.SaveChangesAsync();

                // SERVICES
                if (dto.Services != null && dto.Services.Any())
                {
                    foreach (var item in dto.Services)
                    {
                        // Check existing service
                        var existingService = await _context.Service
                            .FirstOrDefaultAsync(x =>
                                x.BusRegId == dto.BusRegId &&
                                x.ServSubcatId == item.ServSubcatId &&
                                x.ServiceName == item.ServiceName);

                        if (existingService != null)
                        {
                            // UPDATE
                            existingService.ServiceDescription = item.ServiceDescription;
                            existingService.InspectionFee = item.InspectionFee;
                            existingService.StartingPrice = item.StartingPrice;
                            existingService.EstimatedDuration = item.EstimatedDuration;

                            _context.Service.Update(existingService);
                        }
                        else
                        {
                            // INSERT
                            var newService = new Service
                            {
                                BusRegId = dto.BusRegId,
                                BusServId = dto.BusServId,
                                ServSubcatId = item.ServSubcatId,
                                ServiceName = item.ServiceName,
                                ServiceDescription = item.ServiceDescription,
                                InspectionFee = item.InspectionFee,
                                StartingPrice = item.StartingPrice,
                                EstimatedDuration = item.EstimatedDuration
                            };

                            _context.Service.Add(newService);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // get business service details with busregid

        public async Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId)
        {
            return await _context.BusinessRegisters
                .FirstOrDefaultAsync(x => x.BusRegId == busRegId);
        }

        public async Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId)
        {
            var business = await _context.BusinessRegisters
                .FirstOrDefaultAsync(x => x.BusRegId == busRegId);

            if (business == null)
                return null;

            var profile = await _context.ServiceProfiles
                .FirstOrDefaultAsync(x => x.BusRegId == busRegId);

            var services = await _context.Service
                .Where(x => x.BusRegId == busRegId)
                .ToListAsync();

            var result = new ServiceProfileDetailsDto
            {
                BusRegId = business.BusRegId,
                BusinessName = business.BusinessName,
                BusinessMobileNo = business.BusMobileNo,
                BusinessEmail = business.BusEmail,

                BusServId = profile?.BusServId ?? 0,
                YearsOfExperience = profile?.YearsOfExperience,
                GovtIdDocument = profile?.GovtIdDocument,
                ProfessionalLicense = profile?.ProfessionalLicense,
                ServiceAvailableLocations = profile?.ServiceAvailableLocations,
                WorkingDays = profile?.WorkingDays,
                WorkingStartTime = profile?.WorkingStartTime,
                WorkingEndTime = profile?.WorkingEndTime,
                ServiceLogo = profile?.ServiceLogo,
                ServiceBanner = profile?.ServiceBanner,

                Services = services.Select(x => new ServiceItemDto
                {
                    ServiceId = x.ServiceId,
                    ServSubcatId = x.ServSubcatId,
                    ServiceName = x.ServiceName,
                    ServiceDescription = x.ServiceDescription,
                    InspectionFee = x.InspectionFee,
                    StartingPrice = x.StartingPrice,
                    EstimatedDuration = x.EstimatedDuration
                }).ToList()
            };

            return result;
        }
    }
    
}