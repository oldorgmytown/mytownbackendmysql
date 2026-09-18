using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_travel_plans")]
    public class TransporterTravelPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("plan_id")]
        public int PlanId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        // Route

        // =========================================================
        // START LOCATION
        // =========================================================

        [Required]
        [Column("start_town", TypeName = "varchar(100)")]
        public string StartTown { get; set; }

        [Required]
        [Column("start_city", TypeName = "varchar(100)")]
        public string StartCity { get; set; }

        [Required]
        [Column("start_state", TypeName = "varchar(100)")]
        public string StartState { get; set; }

        [Required]
        [Column("start_country", TypeName = "varchar(100)")]
        public string StartCountry { get; set; }

        // =========================================================
        // DESTINATION LOCATION
        // =========================================================

        [Required]
        [Column("destination_town", TypeName = "varchar(100)")]
        public string DestinationTown { get; set; }

        [Required]
        [Column("destination_city", TypeName = "varchar(100)")]
        public string DestinationCity { get; set; }

        [Required]
        [Column("destination_state", TypeName = "varchar(100)")]
        public string DestinationState { get; set; }

        [Required]
        [Column("destination_country", TypeName = "varchar(100)")]
        public string DestinationCountry { get; set; }

        //[Required]
        //[Column("start_location", TypeName = "varchar(300)")]
        //public string StartLocation { get; set; }

        //[Required]
        //[Column("destination", TypeName = "varchar(300)")]
        //public string Destination { get; set; }

        [Column("preferred_route", TypeName = "varchar(200)")]
        public string PreferredRoute { get; set; }

        [Column("distance_km", TypeName = "decimal(10,2)")]
        public decimal? DistanceKm { get; set; }

        // Schedule
        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("arrival_date")]
        public DateTime ArrivalDate { get; set; }

        // Vehicle
        [Required]
        [Column("vehicle_type", TypeName = "varchar(50)")]
        public string VehicleType { get; set; } // Car / TwoWheeler / Bus / Train / Airplane

        [Column("vehicle_registration", TypeName = "varchar(50)")]
        public string VehicleRegistration { get; set; }

        [Column("vehicle_name", TypeName = "varchar(100)")]
        public string VehicleName { get; set; }

        // Capacity
        [Column("max_weight_kg", TypeName = "decimal(10,2)")]
        public decimal MaxWeightKg { get; set; }

        [Column("package_size_l", TypeName = "decimal(10,2)")]
        public decimal? PackageSizeL { get; set; }

        [Column("package_size_w", TypeName = "decimal(10,2)")]
        public decimal? PackageSizeW { get; set; }

        [Column("package_size_h", TypeName = "decimal(10,2)")]
        public decimal? PackageSizeH { get; set; }

        [Column("number_of_packages")]
        public int NumberOfPackages { get; set; }

        [Column("accepts_fragile")]
        public bool AcceptsFragile { get; set; } = false;

        [Column("accepts_perishable")]
        public bool AcceptsPerishable { get; set; } = false;

        // Communication
        [Column("preferred_contact", TypeName = "varchar(20)")]
        public string PreferredContact { get; set; } = "Chat"; // Chat / Call

        [Column("language_preference", TypeName = "varchar(50)")]
        public string LanguagePreference { get; set; } = "English";

        [Column("notify_new_orders")]
        public bool NotifyNewOrders { get; set; } = true;

        [Column("notify_payments")]
        public bool NotifyPayments { get; set; } = true;

        // Status
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("plan_status", TypeName = "varchar(50)")]
        public string PlanStatus { get; set; } = "Available"; // Available / OnTrip / Completed / Cancelled

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }

        public ICollection<TransporterDeliveryRequest> DeliveryRequests { get; set; }
    }
}