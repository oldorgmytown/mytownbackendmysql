using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("wishlist")]
    public class Wishlist
    {
        [Key]
        [Column("wishlist_id")]
        public int WishlistId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("sku_id")]
        public int SkuId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("buscat_id")]
        public int BuscatId { get; set; }

        [Required]
        [Column("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
