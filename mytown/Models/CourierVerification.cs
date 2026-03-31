using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_verifications")]
    public class CourierVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey(nameof(CourierService))]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [Required]
        [Column("verification_token")]
        public string VerificationToken { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual CourierService CourierService { get; set; }
    }
}