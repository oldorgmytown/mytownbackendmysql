using System.ComponentModel.DataAnnotations;

namespace mytown.DTOs
{
    // ---------- Responses ----------

    public class EnquiryFormDataDto
    {
        public int ServiceProfileId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLocation { get; set; }

        public List<EnquiryServiceOptionDto> Services { get; set; } = new();
        public List<string> Locations { get; set; } = new();     // "Location Selection" dropdown
        public List<string> WorkingDays { get; set; } = new();   // e.g. Mon, Tue ...

        public string? WorkingStartTime { get; set; }            // "09:00"
        public string? WorkingEndTime { get; set; }              // "20:00"
        public int SlotMinutes { get; set; }

        public string MinDate { get; set; } = string.Empty;      // yyyy-MM-dd
        public string MaxDate { get; set; } = string.Empty;      // yyyy-MM-dd
    }

    public class EnquiryServiceOptionDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal? StartingPrice { get; set; }
        public string? EstimatedDuration { get; set; }
    }

    public class AvailableDatesDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<string> AvailableDates { get; set; } = new();   // yyyy-MM-dd
    }

    public class AvailableSlotsDto
    {
        public string Date { get; set; } = string.Empty;            // yyyy-MM-dd
        public List<AvailableSlotDto> Slots { get; set; } = new();
    }

    public class AvailableSlotDto
    {
        public string Time { get; set; } = string.Empty;   // "11:30"  (send this back when saving)
        public string Label { get; set; } = string.Empty;  // "11:30 AM"
    }

    // ---------- Request ----------

    public class CreateServiceEnquiryDto
    {
        // TODO: once auth is wired, read this from the JWT claim instead of the body.
        [Required]
        public int ShopperRegId { get; set; }

        [Required]
        public int ServiceProfileId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Range(1, 1000)]
        public int NumberOfUnits { get; set; } = 1;

        [Required]
        public DateTime PreferredDate { get; set; }

        [Required]
        public string PreferredTimeSlot { get; set; } = string.Empty;   // "HH:mm"

        [Required]
        [StringLength(150)]
        public string ServiceLocation { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        // Send only when the shopper used "Change". Omit / null = default address.
        public int? ShopperAddressId { get; set; }
    }

    // ---------- Internal (repo -> service) ----------

    public class BookedSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }

    public enum EnquiryError
    {
        None = 0,
        NotFound,
        BadRequest,
        Conflict
    }

    public class EnquiryResult
    {
        public bool Success { get; set; }
        public EnquiryError Error { get; set; } = EnquiryError.None;
        public string Message { get; set; } = string.Empty;
        public int? EnquiryId { get; set; }

        public static EnquiryResult Ok(int enquiryId, string message) =>
            new() { Success = true, EnquiryId = enquiryId, Message = message };

        public static EnquiryResult Fail(EnquiryError error, string message) =>
            new() { Success = false, Error = error, Message = message };
    }

    public class EnquiryNotificationInfo
    {
        public string? BusinessEmail { get; set; }   // from business register table
        public string? BusinessName { get; set; }

        public string? ShopperName { get; set; }     // from shopper register table
        public string? ShopperPhone { get; set; }
        public string? ShopperAddress { get; set; }  // selected address, else default address
    }

    public class ServiceEnquiryDto
    {
        public int ServiceEnquiryId { get; set; }
        public DateTime EnquiryDate { get; set; }

        public string? BusinessName { get; set; }
        public string? ServiceName { get; set; }
        public string? Priority { get; set; }

        public int Persons { get; set; }
        public DateTime PreferredDate { get; set; }
        public string? TimeSlot { get; set; }          // e.g. "11:30 AM"
        public string? ServiceLocation { get; set; }
        public string? CustomerMessage { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
    }
}