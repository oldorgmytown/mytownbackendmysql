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
        public int? OrderId { get; set; }

        [Column("store_order_id")]
        public int? StoreOrderId { get; set; }

        [ForeignKey(nameof(StoreOrderId))]
        public StoreOrder? StoreOrder { get; set; }

        // ── NEW: human-readable code shown on both sides e.g. DEL-4821 ──
        [Column("delivery_code", TypeName = "varchar(20)")]
        public string DeliveryCode { get; set; } = string.Empty;

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

        // Status flow: Assigned → ReachedPickup → PickedUp → InTransit → Delivered
        // (No Pending / Accepted steps — request is auto-assigned on creation)
        [Column("delivery_status", TypeName = "varchar(50)")]
        public string DeliveryStatus { get; set; } = "Assigned";

        // ── NEW: when the request was auto-assigned (set at creation) ──
        [Column("assigned_at")]
        public DateTime? AssignedAt { get; set; }

        // Existing timestamp columns — kept exactly as-is
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

