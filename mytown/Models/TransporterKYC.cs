using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_kyc")]
    public class TransporterKYC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("kyc_id")]
        public int KycId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [Required]
        [Column("document_type", TypeName = "varchar(50)")]
        public string DocumentType { get; set; } // AadharCard / Passport / DrivingLicense

        [Required]
        [Column("document_number", TypeName = "varchar(100)")]
        public string DocumentNumber { get; set; }

        [Column("document_file_name", TypeName = "varchar(255)")]
        public string DocumentFileName { get; set; }

        [Column("kyc_status", TypeName = "varchar(50)")]
        public string KycStatus { get; set; } = "Pending"; // Pending / Approved / Rejected

        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }
    }
}