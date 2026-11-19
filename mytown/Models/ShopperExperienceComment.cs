using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("shopper_experience_comments")]
    public class ShopperExperienceComment
    {
        [Key]
        [Column("shopper_experience_comment_id")]
        public int ShopperExperienceCommentId { get; set; }

        [Column("shopper_experience_id")]
        public int ShopperExperienceId { get; set; }

        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Column("comment_text")]
        public string CommentText { get; set; }

        [Column("is_anonymous")]
        public bool IsAnonymous { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}