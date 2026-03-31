using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("addtocart")]
    public class AddToCart
    {
        [Key]
        [Column("cart_id")]
        public int CartId { get; set; } // Primary key

        [Required]
        [Column("product_id")]
        [JsonPropertyName("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("sku_id")]
        [JsonPropertyName("sku_id")]
        public int SkuId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("buscat_id")]
        public int BuscatId { get; set; }

        [Required]
        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Column("prod_qty")]
        [JsonPropertyName("prod_qty")]
        public int ProdQty { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("product_price")]
        [JsonPropertyName("product_price")]
        public decimal ProductPrice { get; set; }

        [Column("orderstatus")]
        public string orderstatus { get; set; }



    }
}
