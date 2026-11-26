
using System.ComponentModel.DataAnnotations;

namespace mytown.DTOs
{
    // ---------- Responses ----------

    public class EnquiryFormDataDto
    {
        public int BusRegId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;

        public string? BusinessName { get; set; }
        public string? BusinessLocation { get; set; }

        public List<EnquiryServiceOptionDto> Services { get; set; } = new();
        public List<string> Locations { get; set; } = new();
        public List<string> WorkingDays { get; set; } = new();

        public string? WorkingStartTime { get; set; }
        public string? WorkingEndTime { get; set; }
        public int SlotMinutes { get; set; }

        public string MinDate { get; set; } = string.Empty;
        public string MaxDate { get; set; } = string.Empty;
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
        public List<string> AvailableDates { get; set; } = new();
    }

    public class AvailableSlotsDto
    {
        public string Date { get; set; } = string.Empty;
        public List<AvailableSlotDto> Slots { get; set; } = new();
    }

    public class AvailableSlotDto
    {
        public string Time { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    // ---------- Request ----------

    public class CreateServiceEnquiryDto
    {
        [Range(1, int.MaxValue)]
        public int ShopperRegId { get; set; }

        [Range(1, int.MaxValue)]
        public int BusRegId { get; set; }

        [Range(1, int.MaxValue)]
        public int ServiceId { get; set; }

        [Range(1, 1000)]
        public int NumberOfUnits { get; set; } = 1;

        [Required]
        public DateTime PreferredDate { get; set; }

        [Required]
        public string PreferredTimeSlot { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ServiceLocation { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public int? ShopperAddressId { get; set; }
    }

    // ---------- Internal ----------

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
        public string? BusinessEmail { get; set; }
        public string? BusinessName { get; set; }
        public string? ShopperName { get; set; }
        public string? ShopperPhone { get; set; }
        public string? ShopperAddress { get; set; }
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
        public string? TimeSlot { get; set; }
        public string? ServiceLocation { get; set; }
        public string? CustomerMessage { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
    }
}
