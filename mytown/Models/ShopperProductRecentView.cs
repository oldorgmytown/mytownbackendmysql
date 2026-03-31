using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    [Table("shopper_product_recent_view")]
    public class ShopperProductRecentView
    {
        [Key]
        [Column("recent_view_id")]
        public int RecentViewId { get; set; }  // Primary Key

        [ForeignKey("Shopper")]
        [Column("shopper_id")]
        public int ShopperId { get; set; }     // Shopper who viewed

        [ForeignKey("Product")]
        [Column("product_id")]
        public int ProductId { get; set; }     // Product viewed

        [Column("last_viewed_at")]
        public DateTime LastViewedAt { get; set; } = DateTime.UtcNow; // Last viewed time

        [Column("view_count")]
        public int ViewCount { get; set; } = 1; // Number of times viewed

        // Navigation Properties
        public virtual ShopperRegister Shopper { get; set; }
        public virtual Products Product { get; set; }
    }
}
