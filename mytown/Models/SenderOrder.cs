using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_orders")]
    public class SenderOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sender_order_id")]
        public int SenderOrderId { get; set; }

        [Required]
        [Column("sender_id")]
        public int SenderRegId { get; set; }

        // Product
        [Required]
        [Column("product_name")]
        public string ProductName { get; set; }

        [Column("product_cost")]
        public decimal ProductCost { get; set; }

        [Column("package_length")]
        public decimal? PackageLength { get; set; }

        [Column("package_width")]
        public decimal? PackageWidth { get; set; }

        [Column("package_height")]
        public decimal? PackageHeight { get; set; }

        [Column("package_weight")]
        public decimal? PackageWeight { get; set; }

        [Column("is_fragile")]
        public bool IsFragile { get; set; }

        [Column("is_perishable")]
        public bool IsPerishable { get; set; }

        [Column("special_instructions")]
        public string? SpecialInstructions { get; set; }

        // Pickup
        [Required]
        [Column("pickup_address")]
        public string PickupAddress { get; set; }

        [Column("pickup_town")]
        public string PickupTown { get; set; }

        [Column("pickup_city")]
        public string PickupCity { get; set; }

        [Column("pickup_state")]
        public string PickupState { get; set; }

        [Column("pickup_country")]
        public string PickupCountry { get; set; }

        [Column("pickup_pincode")]
        public string PickupPincode { get; set; }

        [Column("pickup_date")]
        public DateTime PickupDate { get; set; }

        [Column("pickup_time")]
        public string PickupTime { get; set; }

        // Receiver
        [Required]
        [Column("receiver_name")]
        public string ReceiverName { get; set; }

        [Required]
        [Column("receiver_phone")]
        public string ReceiverPhone { get; set; }

        [Required]
        [Column("receiver_address")]
        public string ReceiverAddress { get; set; }

        [Column("receiver_town")]
        public string ReceiverTown { get; set; }

        [Column("receiver_city")]
        public string ReceiverCity { get; set; }

        [Column("receiver_state")]
        public string ReceiverState { get; set; }

        [Column("receiver_country")]
        public string ReceiverCountry { get; set; }

        [Column("receiver_pincode")]
        public string ReceiverPincode { get; set; }

        // Status
        [Column("order_status")]
        public string OrderStatus { get; set; } = "Draft";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("transporter_reg_id")]
        public int? TransporterRegId { get; set; }

        [Column("transporter_plan_id")]
        public int? TransporterPlanId { get; set; }
    }
}