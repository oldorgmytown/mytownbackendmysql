
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_sizes")]
    public class ProductSize
    {
        [Key]
        [Column("size_id")]
        [JsonPropertyName("sizeId")]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SubCategory))]
        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("size_name")]
        [JsonPropertyName("sizeName")]
        public string SizeName { get; set; }

        // ✅ New fields for dimensions
        [Column("length")]
        public decimal? Length { get; set; }

        [Column("width")]
        public decimal? Width { get; set; }

        [Column("height")]
        public decimal? Height { get; set; }

        [Column("weight")]
        public decimal? Weight { get; set; }

        // Navigation property
        public virtual ProductSubCategory SubCategory { get; set; }
    }
}
