using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_order_payments")]
    public class SenderOrderPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sender_payment_id")]
        public int SenderPaymentId { get; set; }

        [Column("sender_order_id")]
        public int SenderOrderId { get; set; }

        [Column("stripe_payment_intent_id")]
        public string StripePaymentIntentId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("gst_amount")]
        public decimal GstAmount { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; }

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}