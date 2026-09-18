using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("store_payout")]
    public class StorePayout
    {
        [Key]
        [Column("payout_id")]
        public int PayoutId { get; set; }

        [Required]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [Required]
        [Column("beneficiary_id")]
        [StringLength(100)]
        public string BeneficiaryId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("transfer_id")]
        [StringLength(100)]
        public string TransferId { get; set; }

        [Column("cf_transfer_id")]
        [StringLength(100)]
        public string? CfTransferId { get; set; }

        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }

        [Column("transfer_utr")]
        [StringLength(100)]
        public string? TransferUtr { get; set; }

        [Column("status_description")]
        [StringLength(500)]
        public string? StatusDescription { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(StoreOrderId))]
        public virtual StoreOrder StoreOrder { get; set; }
    }
}