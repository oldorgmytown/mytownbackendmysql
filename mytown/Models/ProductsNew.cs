using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MyTown.Models;   // <-- ADDED: BusinessRegister lives here

namespace mytown.Models
{
    [Table("products_new")]
    public class ProductsNew
    {
        [Key]
        [Column("product_id")]
        [JsonPropertyName("product_id")]
        public long ProductId { get; set; }

        [ForeignKey(nameof(BusinessRegister))]
        [Column("bus_reg_id")]
        [JsonPropertyName("bus_reg_id")]
        public int BusRegId { get; set; }

        [Column("bus_cat_id")]
        [JsonPropertyName("bus_cat_id")]
        public long? BusCatId { get; set; }

        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public long? ProdSubcatId { get; set; }

        [Column("product_group_id")]
        [JsonPropertyName("product_group_id")]
        public long? ProductGroupId { get; set; }

        [Column("prod_type_id")]
        [JsonPropertyName("prod_type_id")]
        public long? ProdTypeId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("product_name")]
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("product_description")]
        [JsonPropertyName("product_description")]
        public string? ProductDescription { get; set; }

        [Column("product_status")]
        [JsonPropertyName("product_status")]
        public string ProductStatus { get; set; } = "ACTIVE";

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relationships

        [JsonIgnore]
        public virtual BusinessRegister? BusinessRegister { get; set; }

        [JsonPropertyName("variants")]
        public virtual ICollection<ProductVariantNew> Variants { get; set; }
            = new List<ProductVariantNew>();
    }
}