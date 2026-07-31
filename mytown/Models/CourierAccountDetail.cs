using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_account_details")]
    public class CourierAccountDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("account_detail_id")]
        public int AccountDetailId { get; set; }

        [Required]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("account_holder_name")]
        public string AccountHolderName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        [Column("bank_name")]
        public string BankName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("account_number")]
        public string AccountNumber { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("ifsc_code")]
        public string IFSCCode { get; set; } = null!;

        [Column("is_terms_accepted")]
        public bool IsTermsAccepted { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CourierId))]
        public virtual CourierService CourierService { get; set; } = null!;
    }
}