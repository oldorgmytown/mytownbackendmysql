using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shopper_post_images")]
    public class ShopperPostImage
    {
        [Key]
        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("image_url")]
        public string ImageUrl { get; set; } = "";

        [Column("sort_order")]
        public byte SortOrder { get; set; }
    }
}