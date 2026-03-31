using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shipping_details")]
    public class ShippingDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("shipping_detail_id")]
        public int ShippingDetailId { get; set; }

        // 🔗 Parent Order
        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        // 🔗 Store-wise shipping (ONE shipment per store)
        [Required]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [ForeignKey(nameof(StoreOrderId))]
        public StoreOrder StoreOrder { get; set; }

        // 🔗 Selected courier branch
        [Required]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public CourierBranch CourierBranch { get; set; }

        // Courier / Shipping type
        [Required]
        [Column("shipping_type", TypeName = "varchar(100)")]
        public string ShippingType { get; set; }   // e.g. DTDC, BlueDart

        // Delivery estimate
        [Required]
        [Column("estimated_days")]
        public int EstimatedDays { get; set; }

        // Strongly recommended
        //[Required]
        [Column("delivered_date")]
        public DateTime? DeliveredDate { get; set; }

        // Cost charged for this store shipment
        [Required]
        [Column("cost", TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }

        // Courier tracking
        [Column("tracking_id", TypeName = "varchar(100)")]
        public string TrackingId { get; set; }

        // Shipping lifecycle
        [Column("shipping_status", TypeName = "varchar(50)")]
        public string ShippingStatus { get; set; } = "Not Shipped";

        [Column("delivery_address", TypeName = "text")]
        public string DeliveryAddress { get; set; } // new

        // Delivery proof uploaded by courier
        [Column("delivery_proof_file_name", TypeName = "varchar(255)")]
        public string? DeliveryProofFileName { get; set; }

    }
}
