using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_verification")]
    public class TransporterVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("transporter_id")]
        public int TransporterId { get; set; }

        [Column("verification_token")]
        public string VerificationToken { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(TransporterId))]
        public TransporterRegister Transporter { get; set; }
    }
}
