using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("ad_payment_orders")]
    public class AdPaymentOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ad_payment_order_id")]
        public int AdPaymentOrderId { get; set; }

        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Column("budget", TypeName = "decimal(12,2)")]
        public decimal Budget { get; set; }

        [Column("fee", TypeName = "decimal(12,2)")]
        public decimal Fee { get; set; }

        [Column("gst", TypeName = "decimal(12,2)")]
        public decimal Gst { get; set; }

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [StringLength(100)]
        [Column("razorpay_order_id")]
        public string? RazorpayOrderId { get; set; }

        [StringLength(100)]
        [Column("razorpay_payment_id")]
        public string? RazorpayPaymentId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("provider")]
        public string Provider { get; set; } = "Razorpay";

        [StringLength(100)]
        [Column("stripe_payment_intent_id")]
        public string? StripePaymentIntentId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Created";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }
    }

    [Table("ad_promotions")]
    public class AdPromotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Draft";

        [Required]
        [StringLength(20)]
        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("content_json", TypeName = "longtext")]
        public string? ContentJson { get; set; }

        [Column("audience_json", TypeName = "longtext")]
        public string? AudienceJson { get; set; }

        [StringLength(1000)]
        [Column("thumbnail")]
        public string? Thumbnail { get; set; }

        [StringLength(1000)]
        [Column("media_url")]
        public string? MediaUrl { get; set; }

        [Required]
        [StringLength(30)]
        [Column("placement")]
        public string Placement { get; set; } = "profile";

        [Column("duration_days")]
        public int DurationDays { get; set; }

        [Column("budget", TypeName = "decimal(12,2)")]
        public decimal Budget { get; set; }

        [Column("fee", TypeName = "decimal(12,2)")]
        public decimal Fee { get; set; }

        [Column("gst", TypeName = "decimal(12,2)")]
        public decimal Gst { get; set; }

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("payment_order_id")]
        public int? PaymentOrderId { get; set; }

        [StringLength(100)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [StringLength(500)]
        [Column("review_note")]
        public string? ReviewNote { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}