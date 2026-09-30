using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("product_review_photos")]
    public class ProductReviewPhoto
    {
        [Key]
        [Column("product_review_photo_id")]
        public int ProductReviewPhotoId { get; set; }

        [Required]
        [Column("product_review_id")]
        public int ProductReviewId { get; set; }

        [Required]
        [Column("photo_path")]
        public string PhotoPath { get; set; }

        [ForeignKey(nameof(ProductReviewId))]
        public virtual ProductReview ProductReview { get; set; }
    }
}
