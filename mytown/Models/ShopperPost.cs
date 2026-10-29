using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shopper_posts")]
    public class ShopperPost
    {
        [Key]
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required]
        [Column("caption")]
        public string Caption { get; set; } = "";

        [Column("bus_reg_id")]
        public int? BusRegId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public List<ShopperPostImage> Images { get; set; } = new();
    }
}