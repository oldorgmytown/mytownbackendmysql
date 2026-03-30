using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_delivery_requests")]
    public class TransporterDeliveryRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("delivery_req_id")]
        public int DeliveryReqId { get; set; }

        [Required]
        [Column("plan_id")]
        public int PlanId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; } // optional link to store order

        // Package info
        [Column("pickup_location", TypeName = "varchar(300)")]
        public string PickupLocation { get; set; }

        [Column("dropoff_location", TypeName = "varchar(300)")]
        public string DropoffLocation { get; set; }

        [Column("package_weight_kg", TypeName = "decimal(10,2)")]
        public decimal PackageWeightKg { get; set; }

        [Column("number_of_packages")]
        public int NumberOfPackages { get; set; }

        [Column("delivery_fee", TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }

        [Column("package_tags", TypeName = "varchar(100)")]
        public string PackageTags { get; set; } // NA / Fragile / Perishable

        // Status flow: Pending -> Accepted -> ReachedPickup -> PickedUp -> InTransit -> Delivered
        [Column("delivery_status", TypeName = "varchar(50)")]
        public string DeliveryStatus { get; set; } = "Pending";

        [Column("accepted_at")]
        public DateTime? AcceptedAt { get; set; }

        [Column("reached_pickup_at")]
        public DateTime? ReachedPickupAt { get; set; }

        [Column("picked_up_at")]
        public DateTime? PickedUpAt { get; set; }

        [Column("in_transit_at")]
        public DateTime? InTransitAt { get; set; }

        [Column("delivered_at")]
        public DateTime? DeliveredAt { get; set; }

        [Column("delivery_proof_file", TypeName = "varchar(255)")]
        public string DeliveryProofFile { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(PlanId))]
        public TransporterTravelPlan TravelPlan { get; set; }

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }

        [ForeignKey(nameof(ShopperRegId))]
        public ShopperRegister ShopperRegister { get; set; }

        // Exception reports
        public ICollection<TransporterExceptionReport> ExceptionReports { get; set; }
    }
}