using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shipping_details")]
    public class ShippingDetails
    {
        [Key]
        [Column("shipping_detail_id")]
        public int ShippingDetailId { get; set; } // Primary Key

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        [Column("order_detail_id")]
        public int OrderDetailId { get; set; } // Foreign Key - OrderDetails

        [Required]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public CourierBranch CourierBranch { get; set; }

        [Required]
        [Column("shipping_type")]
        public string ShippingType { get; set; } // e.g., Courier A, Courier B

        [Required]
        [Column("estimated_days")]
        public int EstimatedDays { get; set; } // Estimated Days for Delivery

        [Required]
        [Column("cost")]
        public decimal Cost { get; set; } // Shipping Cost

        [Column("tracking_id")]
        public string TrackingId { get; set; } // Unique Tracking ID

        [Column("shipping_status")]
        public string ShippingStatus { get; set; } = "Not Shipped";
    }



}
