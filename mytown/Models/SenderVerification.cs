using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_verification")]
    public class SenderVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sender_id")]
        public int SenderId { get; set; }

        [Column("verification_token")]
        public string VerificationToken { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(SenderId))]
        public SenderRegister Sender { get; set; }
    }
}