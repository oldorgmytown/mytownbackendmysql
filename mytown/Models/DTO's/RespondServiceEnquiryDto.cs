using System.ComponentModel.DataAnnotations;

namespace mytown.Models.DTO_s
{
    public class RespondServiceEnquiryDto
    {
        [Required] public int EnquiryId { get; set; }

        // TODO: read from the logged-in business (JWT) instead of the body
        [Required] public int BusRegId { get; set; }

        [Required] public string Action { get; set; } = string.Empty;   // Accepted | Rejected | Rescheduled

        public string? ProviderResponse { get; set; }                   // message to the shopper

        // only for Rescheduled (the new time offered)
        public DateTime? ConfirmedDate { get; set; }
        public string? ConfirmedTimeSlot { get; set; }
    }
    public class ShopperContactDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public class EnquiryResponseEmailDto
    {
        public int ServiceEnquiryId { get; set; }
        public DateTime EnquiryDate { get; set; }
        public string? ShopperName { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLocation { get; set; }
        public string? ServiceName { get; set; }
        public int Persons { get; set; }
        public string? ServiceLocation { get; set; }
        public string Status { get; set; } = string.Empty;       // Accepted | Rejected | Rescheduled
        public string? ProviderResponse { get; set; }
        public DateTime PreferredDate { get; set; }              // what the shopper asked for
        public string? PreferredTimeSlot { get; set; }           // "11:30 AM"
        public DateTime? ConfirmedDate { get; set; }             // Accepted / Rescheduled
        public string? ConfirmedTimeSlot { get; set; }
    }
}
