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
        private const int BookingWindowDays = 60;  // how far ahead a shopper can enquire
        private const int LeadTimeMinutes = 60;    // today's slots must start at least this far ahead

        // enquiry statuses that hold a slot (Rejected / Cancelled free it again)
        private static readonly string[] SlotHoldingStatuses = { "Pending", "Accepted", "Rescheduled" };

        // ------------------------------------------------------------------
        // Form data
        // ------------------------------------------------------------------
        public async Task<EnquiryFormDataDto?> GetEnquiryFormDataAsync(int serviceProfileId)
        {
            var profile = await _repo.GetEnquiryProfileAsync(serviceProfileId);
            if (profile == null) return null;

            var services = await _repo.GetEnquiryServicesAsync(profile.BusRegId, profile.BusServId);
            var today = DateTime.Today;

            return new EnquiryFormDataDto
            {
                ServiceProfileId = profile.ServiceProfileId,
                BusinessName = profile.BusinessName,
                BusinessLocation = profile.BusinessLocation,
                Services = services.Select(s => new EnquiryServiceOptionDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    StartingPrice = s.StartingPrice,
                    EstimatedDuration = s.EstimatedDuration
                }).ToList(),
                Locations = SplitList(profile.ServiceAvailableLocations),
                WorkingDays = SplitList(profile.WorkingDays),
                WorkingStartTime = profile.WorkingStartTime?.ToString(@"hh\:mm"),
                WorkingEndTime = profile.WorkingEndTime?.ToString(@"hh\:mm"),
                SlotMinutes = SlotMinutes,
                MinDate = today.ToString("yyyy-MM-dd"),
                MaxDate = today.AddDays(BookingWindowDays).ToString("yyyy-MM-dd")
            };
        }

        // ------------------------------------------------------------------
        // Calendar: dates that still have at least one free slot
        // ------------------------------------------------------------------
        public async Task<AvailableDatesDto?> GetEnquiryAvailableDatesAsync(int serviceProfileId, int year, int month)
        {
            var profile = await _repo.GetEnquiryProfileAsync(serviceProfileId);
            if (profile == null) return null;

            var now = DateTime.Now;
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var from = monthStart > now.Date ? monthStart : now.Date;
            var windowEnd = now.Date.AddDays(BookingWindowDays);
            var to = monthEnd < windowEnd ? monthEnd : windowEnd;

            var result = new AvailableDatesDto { Year = year, Month = month };
            if (from > to) return result;

            var blocked = await _repo.GetBlockedSlotsAsync(serviceProfileId, from, to);
            var booked = await _repo.GetBookedSlotsAsync(serviceProfileId, from, to, SlotHoldingStatuses);

            for (var d = from; d <= to; d = d.AddDays(1))
            {
                var slots = BuildAvailableSlots(
                    profile, d,
                    blocked.Where(b => b.BlockedDate.Date == d).ToList(),
                    booked.Where(b => b.Date == d).Select(b => b.Time).ToList(),
                    now);

                if (slots.Count > 0)
                    result.AvailableDates.Add(d.ToString("yyyy-MM-dd"));
            }

            return result;
        }

        // ------------------------------------------------------------------
        // Slots for one date
        // ------------------------------------------------------------------
        public async Task<AvailableSlotsDto?> GetEnquiryAvailableSlotsAsync(int serviceProfileId, DateTime date)
        {
            var profile = await _repo.GetEnquiryProfileAsync(serviceProfileId);
            if (profile == null) return null;

            var slots = await GetFreeSlotsAsync(profile, date.Date);

            return new AvailableSlotsDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Slots = slots.Select(t => new AvailableSlotDto
                {
                    Time = t.ToString(@"hh\:mm"),
                    Label = DateTime.Today.Add(t).ToString("hh:mm tt", CultureInfo.InvariantCulture)
                }).ToList()
            };
        }

        // ------------------------------------------------------------------
        // Save enquiry
        // ------------------------------------------------------------------
        public async Task<EnquiryResult> AddServiceEnquiryAsync(CreateServiceEnquiryDto dto)
        {
            if (!TimeSpan.TryParse(dto.PreferredTimeSlot, CultureInfo.InvariantCulture, out var slot))
                return EnquiryResult.Fail(EnquiryError.BadRequest, "Invalid time slot.");

            var date = dto.PreferredDate.Date;

            var profile = await _repo.GetEnquiryProfileAsync(dto.ServiceProfileId);
            if (profile == null)
                return EnquiryResult.Fail(EnquiryError.NotFound, "Service profile not found.");

            // service must belong to this provider
            var service = await _repo.GetEnquiryServiceAsync(dto.ServiceId, profile.BusRegId, profile.BusServId);
            if (service == null)
                return EnquiryResult.Fail(EnquiryError.BadRequest, "Selected service is not offered by this provider.");

            // re-check the slot is still free (calendar may be stale)
            var freeSlots = await GetFreeSlotsAsync(profile, date);
            if (!freeSlots.Contains(slot))
                return EnquiryResult.Fail(EnquiryError.Conflict, "Selected date / time slot is no longer available.");

            var enquiry = new ServiceEnquiry
            {
                ShopperRegId = dto.ShopperRegId,
                ServiceProfileId = profile.ServiceProfileId,
                BusRegId = profile.BusRegId,
                BusServId = profile.BusServId,
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                NumberOfUnits = dto.NumberOfUnits,
                PreferredDate = date,
                PreferredTimeSlot = slot,
                ServiceLocation = dto.ServiceLocation.Trim(),
                Message = dto.Message.Trim(),
                ShopperAddressId = dto.ShopperAddressId,   // null = default address
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            await _repo.AddServiceEnquiryAsync(enquiry);

            // Notify the business owner. The enquiry is already saved, so a mail
            // problem (bad address, SMTP down) must NOT make the shopper's request fail.
            try
            {
                var info = await _repo.GetEnquiryNotificationInfoAsync(enquiry);

                if (info != null && !string.IsNullOrWhiteSpace(info.BusinessEmail))
                {
                    var emailDto = new ServiceEnquiryDto   // your existing email DTO
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
                        Priority = "New"   // the form has no priority field
                    };

                    await _emailService.SendServiceEnquiryToBusinessAsync(info.BusinessEmail, emailDto);
                }
            }
            catch (Exception ex)
            {
                // TODO: use your logger instead of Console
                Console.WriteLine($"Enquiry {enquiry.EnquiryId} saved, but owner email failed: {ex.Message}");
            }

            return EnquiryResult.Ok(enquiry.EnquiryId,
                "Enquiry submitted. Provider availability will be confirmed on response.");
        }

        // ==================================================================
        // Private helpers (enquiry)
        // ==================================================================

        private async Task<List<TimeSpan>> GetFreeSlotsAsync(mytown.Models.ServiceProfile profile, DateTime date)
        {
            var now = DateTime.Now;

            if (date < now.Date || date > now.Date.AddDays(BookingWindowDays))
                return new List<TimeSpan>();

            var blocked = await _repo.GetBlockedSlotsAsync(profile.ServiceProfileId, date, date);
            var booked = await _repo.GetBookedSlotsAsync(profile.ServiceProfileId, date, date, SlotHoldingStatuses);

            return BuildAvailableSlots(profile, date, blocked, booked.Select(b => b.Time).ToList(), now);
        }

        /// Working hours cut into SlotMinutes slots, minus blocked ranges,
        /// slots held by enquiries and slots already in the past.
        private static List<TimeSpan> BuildAvailableSlots(
            mytown.Models.ServiceProfile profile,
            DateTime date,
            List<ServiceBlockedSlot> blockedForDate,
            List<TimeSpan> bookedTimes,
            DateTime now)
        {
            var result = new List<TimeSpan>();

            if (profile.WorkingStartTime == null || profile.WorkingEndTime == null)
                return result;

            if (!IsWorkingDay(profile.WorkingDays, date.DayOfWeek))
                return result;

            // whole day blocked
            if (blockedForDate.Any(b => b.StartTime == null || b.EndTime == null))
                return result;

            var step = TimeSpan.FromMinutes(SlotMinutes);
            var earliest = now.AddMinutes(LeadTimeMinutes);

            for (var t = profile.WorkingStartTime.Value; t + step <= profile.WorkingEndTime.Value; t += step)
            {
                if (date.Date + t < earliest) continue;

                bool isBlocked = blockedForDate.Any(b => t < b.EndTime && t + step > b.StartTime);
                if (isBlocked) continue;

                bool isBooked = bookedTimes.Any(s => t < s + step && t + step > s);
                if (isBooked) continue;

                result.Add(t);
            }

            return result;
        }

        /// Works with "Mon,Tue,Wed", "Monday, Tuesday", "Mon Tue" etc.
        /// A range like "Mon-Sat" is NOT understood. No working days saved = not bookable.
        private static bool IsWorkingDay(string? workingDays, DayOfWeek day)
        {
            if (string.IsNullOrWhiteSpace(workingDays)) return false;

            var key = day.ToString().Substring(0, 3);
            return workingDays
                .Split(new[] { ',', ';', '|', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(p => p.Trim().StartsWith(key, StringComparison.OrdinalIgnoreCase));
        }

        private static List<string> SplitList(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new List<string>();

            return value
                .Split(new[] { ',', ';', '|', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToList();
        }
    }


}

