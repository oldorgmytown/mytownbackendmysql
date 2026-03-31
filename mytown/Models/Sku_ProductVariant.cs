
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("Sku_ProductVariants")]
    public class Sku_ProductVariant
    {
        [Key]
        [Column("sku_id")]
        public int SkuId { get; set; }      // Primary Key for the variant

        [ForeignKey(nameof(Product))]
        [Column("product_id")]
        public int ProductId { get; set; }  // FK → Products

        // Variant-specific attributes
        [StringLength(100)]
        [Column("color")]
        public string? Color { get; set; }


        [Column("size_id")]
        public int? SizeId { get; set; }

        [Range(0, double.MaxValue)]
        [Column("sku_cost")]
        public decimal Sku_Cost { get; set; }

        [Range(0, double.MaxValue)]
        [Column("discount_price")]
        public decimal? DiscountPrice { get; set; }

        [Range(0, double.MaxValue)]
        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        [Column("length")]
        public decimal? Length { get; set; }

        [Range(0, double.MaxValue)]
        [Column("width")]
        public decimal? Width { get; set; }

        [Range(0, double.MaxValue)]
        [Column("height")]
        public decimal? Height { get; set; }

        [Range(0, double.MaxValue)]
        [Column("weight")]
        public decimal? Weight { get; set; }

        [Range(0, 100)]
        [Column("discount")]
        public decimal? Discount { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Products Product { get; set; }

        public virtual ICollection<ProductImage>? Images { get; set; } = new List<ProductImage>();


        [ForeignKey("SizeId")]
        public virtual ProductSize Size { get; set; }
    }
}

