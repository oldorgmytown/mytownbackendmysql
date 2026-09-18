using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("shopper_experience_likes")]
    public class ShopperExperienceLike
    {
        [Key]
        [Column("shopper_experience_like_id")]
        public int ShopperExperienceLikeId { get; set; }

        [Column("shopper_experience_id")]
        public int ShopperExperienceId { get; set; }

        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}