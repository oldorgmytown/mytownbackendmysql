using mytown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("product_reviews")]
    public class ProductReview
    {
        [Key]
        [Column("product_review_id")]
        public int ProductReviewId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required]
        [Column("product_id")]
        public long ProductId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("post_type")]
        public string PostType { get; set; }

        [Column("rating")]
        public decimal? Rating { get; set; }

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("review")]
        public string Review { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Approved";

        [Column("is_anonymous")]
        public bool IsAnonymous { get; set; } = false;

        [Column("verified_purchase")]
        public bool VerifiedPurchase { get; set; } = false;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ShopperRegId))]
        public virtual ShopperRegister ShopperRegister { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual ProductsNew Product { get; set; }

        public virtual ICollection<ProductReviewPhoto> Photos { get; set; }
            = new List<ProductReviewPhoto>();
    }
}