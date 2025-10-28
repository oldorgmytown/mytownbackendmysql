
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("payments")]
    public class Payments
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [Required]
        [Column("amount_paid")]
        public decimal AmountPaid { get; set; }

        [Required]
        [Column("payment_method")]
        public string PaymentMethod { get; set; }

        [Column("payment_status")]
        public string PaymentStatus { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }
    }

}
