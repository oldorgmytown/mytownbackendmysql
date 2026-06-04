using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
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
                    existingProfile.BusinessName = dto.BusinessName;
                    existingProfile.BusinessLocation = dto.BusinessLocation;
                    existingProfile.ServiceDescription = dto.ServiceDescription;
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
                        BusinessName = dto.BusinessName,
                        BusinessLocation = dto.BusinessLocation,
                        ServiceDescription = dto.ServiceDescription,
                        YearsOfExperience = dto.YearsOfExperience,
                        GovtIdDocument = dto.GovtIdDocument,
                        ProfessionalLicense = dto.ProfessionalLicense,
                        ServiceAvailableLocations = dto.ServiceAvailableLocations,
                        WorkingDays = dto.WorkingDays,
                        WorkingStartTime = dto.WorkingStartTime,
                        WorkingEndTime = dto.WorkingEndTime,
                        ServiceLogo = dto.ServiceLogo,
                        ServiceBanner = dto.ServiceBanner,
                        CreatedDate = DateTime.Now,
                        Status = "Submitted"
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
                            existingService.ServiceTypeDescription = item.ServiceTypeDescription;
                            existingService.InspectionFee = item.InspectionFee;
                            existingService.StartingPrice = item.StartingPrice;
                            existingService.EstimatedDuration = item.EstimatedDuration;
                            existingService.ServiceTypeImage = item.ServiceTypeImage;

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
                                ServiceTypeDescription = item.ServiceTypeDescription,
                                InspectionFee = item.InspectionFee,
                                StartingPrice = item.StartingPrice,
                                EstimatedDuration = item.EstimatedDuration,
                                ServiceTypeImage = item.ServiceTypeImage
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
            var services = await _context.Service
              .Where(x => x.BusRegId == busRegId)
              .ToListAsync();

            var profile = await _context.ServiceProfiles
                .FirstOrDefaultAsync(x => x.BusRegId == busRegId);

          

            var result = new ServiceProfileDetailsDto
            {
                BusRegId = business.BusRegId,
                BusinessName = business?.BusinessName ?? string.Empty,
                BusinessLocation = profile?.BusinessLocation ?? string.Empty,
                BusinessMobileNo = business.BusMobileNo,
                BusinessEmail = business.BusEmail,
                ServiceDescription = profile?.ServiceDescription ?? string.Empty,
                BusServId = profile?.BusServId ?? 0,
                YearsOfExperience = profile?.YearsOfExperience ?? 0,
                GovtIdDocument = profile?.GovtIdDocument ?? string.Empty,
                ProfessionalLicense = profile?.ProfessionalLicense ?? string.Empty,
                ServiceAvailableLocations = profile?.ServiceAvailableLocations ?? string.Empty,
                WorkingDays = profile?.WorkingDays ?? string.Empty,
                WorkingStartTime = profile?.WorkingStartTime,
                WorkingEndTime = profile?.WorkingEndTime,
                ServiceLogo = profile?.ServiceLogo ?? string.Empty,
                ServiceBanner = profile?.ServiceBanner ?? string.Empty,

                Services = services.Select(x => new ServiceItemDto
                {
                    ServiceId = x.ServiceId,
                    ServSubcatId = x.ServSubcatId,
                    ServiceName = x.ServiceName,
                    ServiceTypeDescription = x.ServiceTypeDescription,
                    InspectionFee = x.InspectionFee,
                    StartingPrice = x.StartingPrice,
                    EstimatedDuration = x.EstimatedDuration,
                    ServiceTypeImage = x?.ServiceTypeImage ?? string.Empty
                }).ToList()
            };

            return result;
        }

        public async Task<List<BusinessServiceTypesDto>> GetBusinessServiceTypesAsync(int busRegId)
        {
            var result = await (
                from s in _context.Service
                join bs in _context.BusinessServices
                    on s.BusServId equals bs.BusServId
                join sc in _context.ServiceSubCategory
                    on s.ServSubcatId equals sc.ServSubcatId
                where s.BusRegId == busRegId
                group new { bs, sc } by new
                {
                    bs.BusServId,
                    bs.BusinessServiceName
                }
                into g
                select new BusinessServiceTypesDto
                {
                    BusServId = g.Key.BusServId,
                    BusinessServiceName = g.Key.BusinessServiceName,
                    ServiceTypeNames = g.Select(x => x.sc.ServiceTypeName)
                                        .Distinct()
                                        .ToList()
                }
            ).ToListAsync();

            return result;
        }
    }
    
}