using MyTown.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("business_verifications")]
    public class BusinessVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey(nameof(BusinessRegister))]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("verification_token")]
        public string VerificationToken { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual BusinessRegister BusinessRegister { get; set; }
    }
}
