using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("service_enquiries")]
    public class ServiceEnquiry
    {
        [Key]
        [Column("enquiry_id")]
        public int EnquiryId { get; set; }

        // ---- Who / which provider ----
        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }           // shopper who sends the enquiry

        [Required]
        [Column("service_profile_id")]
        public int ServiceProfileId { get; set; }       // Naturals Salon & Spa profile

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("bus_serv_id")]
        public int BusServId { get; set; }

        // ---- Form fields ----
        [Required]
        [Column("service_id")]
        public int ServiceId { get; set; }              // "Service" dropdown (Hair Style)

        [Required]
        [StringLength(150)]
        [Column("service_name")]
        public string ServiceName { get; set; }         // snapshot of name at enquiry time

        [Required]
        [Range(1, int.MaxValue)]
        [Column("number_of_units")]
        public int NumberOfUnits { get; set; } = 1;     // "Number (Persons / Units)" - min 1

        [Required]
        [Column("preferred_date")]
        public DateTime PreferredDate { get; set; }     // 11/24/2026

        [Required]
        [Column("preferred_time_slot")]
        public TimeSpan PreferredTimeSlot { get; set; } // 11:30 AM

        [Required]
        [StringLength(150)]
        [Column("service_location")]
        public string ServiceLocation { get; set; }     // "Jayanagar, Bangalore"

        [Required]
        [Column("message")]
        public string Message { get; set; }

        // ---- Shopper address ----
        // Filled only when the shopper changes the address in the form.
        // NULL = the shopper's default address is used.
        [Column("shopper_address_id")]
        public int? ShopperAddressId { get; set; }

        // ---- Provider response / workflow ----
        // Pending, Accepted, Rejected, Rescheduled, Cancelled, Completed
        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("provider_response")]
        public string? ProviderResponse { get; set; }

        [Column("confirmed_date")]
        public DateTime? ConfirmedDate { get; set; }

        [Column("confirmed_time_slot")]
        public TimeSpan? ConfirmedTimeSlot { get; set; }

        [Column("responded_date")]
        public DateTime? RespondedDate { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
    }
}