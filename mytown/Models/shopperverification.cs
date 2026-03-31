using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("ShopperVerification")]
    public class ShopperVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("shopper_id")]
        public int ShopperId { get; set; }

        [Column("verification_token")]
        public string VerificationToken { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ShopperId))]
        public ShopperRegister Shopper { get; set; }
    }
}
