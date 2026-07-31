using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("business_account_details")]
    public class BusinessAccountDetail
    {
        [Key]
        [Column("account_detail_id")]
        public int AccountDetailId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("account_holder_name")]
        public string AccountHolderName { get; set; }

        [Required]
        [StringLength(150)]
        [Column("bank_name")]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(20)]
        [Column("ifsc_code")]
        public string IFSCCode { get; set; }
       

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(BusRegId))]
        public virtual BusinessRegister BusinessRegister { get; set; }
    }
}