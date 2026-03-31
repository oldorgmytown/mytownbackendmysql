using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_bank_details")]
    public class TransporterBankDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("bank_detail_id")]
        public int BankDetailId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [Required]
        [Column("bank_name", TypeName = "varchar(100)")]
        public string BankName { get; set; }

        [Required]
        [Column("account_number", TypeName = "varchar(50)")]
        public string AccountNumber { get; set; }

        [Required]
        [Column("branch_name", TypeName = "varchar(100)")]
        public string BranchName { get; set; }

        [Required]
        [Column("ifsc_code", TypeName = "varchar(20)")]
        public string IfscCode { get; set; }

        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }
    }
}