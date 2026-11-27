using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;
using System.Globalization;

namespace mytown.Services.Implementations
{
    public class ServicesProfile : IServicesProfile
    {
        private readonly IBusinessServiceRepository _repo;
        private readonly IEmailService _emailService;
        public ServicesProfile(IBusinessServiceRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public async Task<List<mytown.Models.BusinessService>> GetAllServicesAsync()
        {
            return await _repo.GetAllServicesAsync();


        }

        public async Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId)
        {
            return await _repo.GetByBusServIdAsync(busServId);
        }
        public async Task<bool> AddServiceProfileAsync(CreateServiceProfileDto dto)
        {
            return await _repo.AddOrUpdateServiceProfileAsync(dto);
        }

        public async Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId)
        {
            return await _repo.GetByBusRegIdAsync(busRegId);
        }

        public async Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId)
        {
            return await _repo.GetServiceProfileDetailsAsync(busRegId);
        }

        public async Task<List<BusinessServiceTypesDto>> GetBusinessServiceTypesAsync(int busRegId)
        {
            return await _repo.GetBusinessServiceTypesAsync(busRegId);
        }

        public async Task<List<Service>> GetServicesByBusRegIdAsync(int busRegId)
        {
            return await _repo.GetServicesByBusRegIdAsync(busRegId);
        }

        public async Task<bool> UpdateServiceAsync(UpdateServiceDto dto)
        {
            return await _repo.UpdateServiceAsync(dto);
        }



        // ---------- Service enquiry : booking rules (change as needed) ----------
        private const int SlotMinutes = 30;        // length of one slot
        private const int BookingDays = 60;  // how far ahead a shopper can enquire
        private const int LeadTimeMinutes = 60;    // today's slots must start at least this far ahead

        // enquiry statuses that hold a slot (Rejected / Cancelled free it again)
        private static readonly string[] SlotHoldingStatuses = { "Pending", "Accepted", "Rescheduled" };

        // ------------------------------------------------------------------
        // Form data
        // ------------------------------------------------------------------
        private async Task<(Service? service, ServiceProfile? profile)> GetServiceProfileAsync(
            int busRegId, int serviceId)
        {
            var service = await _repo.GetEnquiryServiceAsync(serviceId, busRegId);

            if (service == null)
                return (null, null);

            var profile = await _repo.GetEnquiryProfileAsync(
                busRegId, service.BusServId);

            return (service, profile);
        }

        public async Task<EnquiryFormDataDto?> GetEnquiryFormDataAsync(
            int busRegId, int serviceId)
        {
            var (service, profile) = await GetServiceProfileAsync(busRegId, serviceId);

            if (service == null || profile == null)
                return null;

            return new EnquiryFormDataDto
            {
                BusRegId = busRegId,
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                BusinessName = profile.BusinessName,
                BusinessLocation = profile.BusinessLocation,

                Services = new List<EnquiryServiceOptionDto>
                {
                    new()
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName,
                        StartingPrice = service.StartingPrice,
                        EstimatedDuration = service.EstimatedDuration
                    }
                },

                Locations = SplitList(profile.ServiceAvailableLocations),
                WorkingDays = SplitList(profile.WorkingDays),
                WorkingStartTime = profile.WorkingStartTime?.ToString(@"hh\:mm"),
                WorkingEndTime = profile.WorkingEndTime?.ToString(@"hh\:mm"),
                SlotMinutes = SlotMinutes,
                MinDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
                MaxDate = DateTime.Today.AddDays(BookingDays).ToString("yyyy-MM-dd")
            };
        }

        public async Task<AvailableDatesDto?> GetEnquiryAvailableDatesAsync(
            int busRegId, int serviceId, int year, int month)
        {
            var (_, profile) = await GetServiceProfileAsync(busRegId, serviceId);

            if (profile == null)
                return null;

            if (month < 1 || month > 12)
                return null;

            var today = DateTime.Today;
            var minDate = today.AddMinutes(LeadTimeMinutes);
            var maxDate = today.AddDays(BookingDays);

            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var startDate = monthStart < today.Date ? today.Date : monthStart;
            var endDate = monthEnd > maxDate.Date ? maxDate.Date : monthEnd;

            var result = new AvailableDatesDto
            {
                Year = year,
                Month = month
            };

            if (startDate > endDate)
                return result;

            var blocked = await _repo.GetBlockedSlotsAsync(
                profile.ServiceProfileId, startDate, endDate);

            var booked = await _repo.GetBookedSlotsAsync(
                profile.ServiceProfileId, startDate, endDate);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var slots = BuildAvailableSlots(profile, date, blocked, booked, minDate);

                if (slots.Count > 0)
                    result.AvailableDates.Add(date.ToString("yyyy-MM-dd"));
            }

            return result;
        }

        public async Task<AvailableSlotsDto?> GetEnquiryAvailableSlotsAsync(
            int busRegId, int serviceId, DateTime date)
        {
            var (_, profile) = await GetServiceProfileAsync(busRegId, serviceId);

            if (profile == null)
                return null;

            var today = DateTime.Today;
            var minDate = today.AddMinutes(LeadTimeMinutes);
            var maxDate = today.AddDays(BookingDays);

            var result = new AvailableSlotsDto
            {
                Date = date.ToString("yyyy-MM-dd")
            };

            if (date.Date < today.Date || date.Date > maxDate.Date)
                return result;

            var blocked = await _repo.GetBlockedSlotsAsync(
                profile.ServiceProfileId, date.Date, date.Date);

            var booked = await _repo.GetBookedSlotsAsync(
                profile.ServiceProfileId, date.Date, date.Date);

            result.Slots = BuildAvailableSlots(
                profile, date.Date, blocked, booked, minDate);

            return result;
        }

        private List<AvailableSlotDto> BuildAvailableSlots(
            ServiceProfile profile,
            DateTime date,
            List<ServiceBlockedSlot> blocked,
            List<BookedSlotDto> booked,
            DateTime minDate)
        {
            var slots = new List<AvailableSlotDto>();

            if (profile.WorkingStartTime == null ||
                profile.WorkingEndTime == null ||
                !IsWorkingDay(profile.WorkingDays, date.DayOfWeek))
                return slots;

            var start = profile.WorkingStartTime.Value;
            var end = profile.WorkingEndTime.Value;

            for (var time = start; time.Add(TimeSpan.FromMinutes(SlotMinutes)) <= end;
                 time = time.Add(TimeSpan.FromMinutes(SlotMinutes)))
            {
                var slotStart = date.Date.Add(time);
                var slotEnd = slotStart.AddMinutes(SlotMinutes);

                if (slotStart < minDate)
                    continue;

                var isBlocked = blocked.Any(b =>
                    b.BlockedDate.Date == date.Date &&
                    (b.StartTime == null ||
                     b.EndTime == null ||
                     (time < b.EndTime.Value &&
                      time.Add(TimeSpan.FromMinutes(SlotMinutes)) > b.StartTime.Value)));

                var isBooked = booked.Any(b =>
                    b.Date.Date == date.Date &&
                    b.Time == time);

                if (isBlocked || isBooked)
                    continue;

                slots.Add(new AvailableSlotDto
                {
                    Time = time.ToString(@"hh\:mm"),
                    Label = DateTime.Today.Add(time).ToString("hh:mm tt")
                });
            }

            return slots;
        }

        private bool IsWorkingDay(string? workingDays, DayOfWeek day)
        {
            return SplitList(workingDays).Any(d =>
                d.Equals(day.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private List<string> SplitList(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
        }

        public async Task<EnquiryResult> AddServiceEnquiryAsync(
            CreateServiceEnquiryDto dto)
        {
            var (service, profile) = await GetServiceProfileAsync(
                dto.BusRegId, dto.ServiceId);

            if (service == null || profile == null)
                return EnquiryResult.Fail(
                    EnquiryError.NotFound, "Selected service or profile not found.");

            if (!TimeSpan.TryParseExact(
                    dto.PreferredTimeSlot,
                    new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss" },
                    CultureInfo.InvariantCulture,
                    out var preferredTime))
            {
                return EnquiryResult.Fail(
                    EnquiryError.BadRequest, "Invalid time slot.");
            }

            var date = dto.PreferredDate.Date;
            var minDate = DateTime.Now.AddMinutes(LeadTimeMinutes);
            var maxDate = DateTime.Today.AddDays(BookingDays);

            if (date < DateTime.Today || date > maxDate)
                return EnquiryResult.Fail(
                    EnquiryError.BadRequest, "Selected date is outside the booking period.");

            var blocked = await _repo.GetBlockedSlotsAsync(
                profile.ServiceProfileId, date, date);

            var booked = await _repo.GetBookedSlotsAsync(
                profile.ServiceProfileId, date, date);

            var availableSlots = BuildAvailableSlots(
                profile, date, blocked, booked, minDate);

            if (!availableSlots.Any(s =>
                    TimeSpan.TryParse(s.Time, out var slotTime) &&
                    slotTime == preferredTime))
            {
                return EnquiryResult.Fail(
                    EnquiryError.Conflict, "Selected time slot is no longer available.");
            }

            var enquiry = new ServiceEnquiry
            {
                ShopperRegId = dto.ShopperRegId,
                ServiceProfileId = profile.ServiceProfileId,
                BusRegId = dto.BusRegId,
                BusServId = service.BusServId,
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                NumberOfUnits = dto.NumberOfUnits,
                PreferredDate = date,
                PreferredTimeSlot = preferredTime,
                ServiceLocation = dto.ServiceLocation,
                Message = dto.Message,
                ShopperAddressId = dto.ShopperAddressId,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            await _repo.AddServiceEnquiryAsync(enquiry);

            // Send enquiry notification to business owner
            try
            {
                var info = await _repo.GetEnquiryNotificationInfoAsync(enquiry);

                if (info != null && !string.IsNullOrWhiteSpace(info.BusinessEmail))
                {
                    var emailDto = new ServiceEnquiryDto
                    {
                        ServiceEnquiryId = enquiry.EnquiryId,
                        EnquiryDate = enquiry.CreatedDate,
                        BusinessName = profile.BusinessName ?? info.BusinessName,
                        ServiceName = enquiry.ServiceName,
                        Persons = enquiry.NumberOfUnits,
                        PreferredDate = enquiry.PreferredDate,
                        TimeSlot = DateTime.Today.Add(enquiry.PreferredTimeSlot)
                            .ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        ServiceLocation = enquiry.ServiceLocation,
                        CustomerMessage = enquiry.Message,
                        CustomerName = info.ShopperName,
                        CustomerPhone = info.ShopperPhone,
                        CustomerAddress = info.ShopperAddress,
                        Priority = "New"
                    };

                    await _emailService.SendServiceEnquiryToBusinessAsync(
                        info.BusinessEmail, emailDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Enquiry {enquiry.EnquiryId} saved, but owner email failed: {ex.Message}");
            }

            return EnquiryResult.Ok(
                enquiry.EnquiryId,
                "Enquiry submitted. Provider availability will be confirmed on response.");

            
        }

        public async Task<BusinessServiceEnquiriesResponseDto> GetBusinessServiceEnquiriesAsync(
    int busRegId, string? status, int? serviceId, string? serviceType)
        {
            var enquiries = await _repo.GetBusinessServiceEnquiriesAsync(busRegId);

            var response = new BusinessServiceEnquiriesResponseDto
            {
                TotalCount = enquiries.Count,
                NewCount = enquiries.Count(x => x.Status == "New"),
                PendingCount = enquiries.Count(x => x.Status == "Pending"),
                RespondedCount = enquiries.Count(x => x.Status == "Responded"),
                CompletedCount = enquiries.Count(x => x.Status == "Completed")
            };

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                enquiries = enquiries.Where(x => x.Status.Equals(
                    status, StringComparison.OrdinalIgnoreCase)).ToList();

            if (serviceId.HasValue)
                enquiries = enquiries.Where(x => x.ServiceId == serviceId.Value).ToList();

            if (!string.IsNullOrWhiteSpace(serviceType))
                enquiries = enquiries.Where(x => x.ServiceName.Contains(
                    serviceType, StringComparison.OrdinalIgnoreCase)).ToList();

            response.Enquiries = enquiries
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return response;
        }
    }


}

