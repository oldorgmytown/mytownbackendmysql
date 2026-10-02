using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_payout")]
    public class CourierPayout
    {
        [Key]
        [Column("payout_id")]
        public int PayoutId { get; set; }

        [Required]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [Required]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("beneficiary_id")]
        public string BeneficiaryId { get; set; } = null!;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        [Column("transfer_id")]
        public string TransferId { get; set; } = null!;

        [StringLength(100)]
        [Column("cf_transfer_id")]
        public string? CfTransferId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; } = null!;

        [StringLength(100)]
        [Column("transfer_utr")]
        public string? TransferUtr { get; set; }

        [StringLength(500)]
        [Column("status_description")]
        public string? StatusDescription { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
    }
}