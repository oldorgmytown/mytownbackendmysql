using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("products")]
    public class Products
    {
        [Key]
        [Column("product_id")]
        [JsonPropertyName("product_id")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(BusinessRegister))]
        [Column("BusRegId")]
        [JsonPropertyName("BusRegId")]
        public int BusRegId { get; set; }

        [Column("BuscatId")]
        [JsonPropertyName("BuscatId")]
        public int BuscatId { get; set; }

        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("product_name")]
        [JsonPropertyName("product_name")]
        public string? ProductName { get; set; }

        [StringLength(100)]
        [Column("product_subject")]
        [JsonPropertyName("product_subject")]
        public string? ProductSubject { get; set; }

        [Required]
        [StringLength(500)]
        [Column("product_description")]
        [JsonPropertyName("product_description")]
        public string? ProductDescription { get; set; }

        [Column("product_image")]
        [JsonPropertyName("product_image")]
        public string? ProductImage { get; set; }

        [Column("supplier_name")]
        [JsonPropertyName("supplier_name")]
        public string? SupplierName { get; set; }

        [Column("ProductTypeId")]
        [JsonPropertyName("ProductTypeId")]
        public int? ProductTypeId { get; set; }

        [Column("FabricId")]
        [JsonPropertyName("FabricId")]
        public int? FabricId { get; set; }

        [Column("DesignId")]
        [JsonPropertyName("DesignId")]
        public int? DesignId { get; set; }

        // Relationships
        [ForeignKey(nameof(ProductTypeId))]
        [JsonPropertyName("product_type")]
        public virtual ProductType? ProductType { get; set; }

        [ForeignKey(nameof(FabricId))]
        [JsonPropertyName("fabric")]
        public virtual Fabric? Fabric { get; set; }

        [ForeignKey(nameof(DesignId))]
        [JsonPropertyName("design")]
        public virtual Design? Design { get; set; }

      
        [JsonIgnore]
        public virtual BusinessRegister? BusinessRegister { get; set; }

        [JsonPropertyName("images")]
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        [JsonPropertyName("sku_product_variants")]
        public virtual ICollection<Sku_ProductVariant> Sku_ProductVariants { get; set; } = new List<Sku_ProductVariant>();
    }
}
