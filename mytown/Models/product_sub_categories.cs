using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_sub_categories")]
    public class ProductSubCategory
    {
        [Key]
        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Column("bus_cat_id")]
        [JsonPropertyName("buscatId")]
        public int BuscatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("prod_subcat_name")]
        [JsonPropertyName("prod_subcat_name")]
        public string ProdSubcatName { get; set; }

        [Column("prod_subcat_image")]
        [JsonPropertyName("prod_subcat_image")]
        public string? ProdSubcatImage { get; set; }
    }
}
