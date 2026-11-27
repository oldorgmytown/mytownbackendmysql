using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_variants")]
    public class ProductVariantNew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sku_id")]
        public long SkuId { get; set; }

        [ForeignKey(nameof(Product))]
        [Column("product_id")]
        [JsonPropertyName("product_id")]
        public long ProductId { get; set; }

        [Column("stock_quantity")]
        [JsonPropertyName("stock_quantity")]
        public decimal StockQuantity { get; set; } = 0;

        [Column("weight")]
        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [StringLength(30)]
        [Column("measurement_unit")]
        [JsonPropertyName("measurement_unit")]
        public string? MeasurementUnit { get; set; }

        [Column("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Column("discount")]
        [JsonPropertyName("discount")]
        public decimal Discount { get; set; } = 0;

        [Column("discount_price")]
        [JsonPropertyName("discount_price")]
        public decimal? DiscountPrice { get; set; }

        [StringLength(200)]
        [Column("brand")]
        [JsonPropertyName("brand")]
        public string? Brand { get; set; }

       

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // Parent Product

        [JsonIgnore]
        public virtual ProductsNew? Product { get; set; }


        // Variant Attributes

        [JsonPropertyName("attributes")]
        public virtual ICollection<ProductVariantAttributeNew> Attributes { get; set; }
            = new List<ProductVariantAttributeNew>();

        [JsonPropertyName("images")]
        public virtual ICollection<ProductVariantImageNew> Images { get; set; }
    = new List<ProductVariantImageNew>();
    }
}