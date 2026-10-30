using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models


{


    [PrimaryKey(nameof(PostId), nameof(LikedByShopperId))]
    [Table("shopper_post_likes")]
   // [Table("shopper_post_likes")]
    public class ShopperPostLike
    {
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("liked_by_shopper_id")]
        public int LikedByShopperId { get; set; }

        [Column("liked_at")]
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}