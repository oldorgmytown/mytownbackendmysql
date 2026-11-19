using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("order_transport_legs")]
    public class OrderTransportLeg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_transport_leg_id")]
        public int OrderTransportLegId { get; set; }

        // Parent shipment (one shipping_details row per store order)
        [Required]
        [Column("shipping_detail_id")]
        public int ShippingDetailId { get; set; }

        [ForeignKey(nameof(ShippingDetailId))]
        public ShippingDetails ShippingDetails { get; set; }

        // Kept as plain ids (no navigation) to avoid multiple cascade path errors
        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [Required]
        [Column("leg_number")]
        public int LegNumber { get; set; }              // 1, 2, 3

        [Column("is_final_leg")]
        public bool IsFinalLeg { get; set; }            // true only for the leg that delivers to the shopper

        [Required]
        [Column("transporter_plan_id")]
        public int TransporterPlanId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [Column("from_pin", TypeName = "varchar(20)")]
        public string FromPin { get; set; } = string.Empty;

        [Column("to_pin", TypeName = "varchar(20)")]
        public string ToPin { get; set; } = string.Empty;

        [Column("planned_start")]
        public DateTime PlannedStart { get; set; }

        [Column("planned_arrival")]
        public DateTime PlannedArrival { get; set; }

        // Assigned, Picked up, In transit, Delivered
        [Column("leg_status", TypeName = "varchar(50)")]
        public string LegStatus { get; set; } = "Assigned";

        // Hub at the end of this leg: New order, Reached hub, Handed over
        [Column("hub_status", TypeName = "varchar(50)")]
        public string HubStatus { get; set; } = "New order";

        [Column("delivered_at")]
        public DateTime? DeliveredAt { get; set; }

        // For now Cost / number of legs (50 each). Real split can come later.
        [Column("leg_cost", TypeName = "decimal(10,2)")]
        public decimal LegCost { get; set; }

        [Column("is_paid")]
        public bool IsPaid { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }
    }
}
