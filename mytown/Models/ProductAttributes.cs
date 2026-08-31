using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_attributes")]
    public class ProductAttributes
    {
        [Key]
        [Column("attribute_id")]
        [JsonPropertyName("attribute_id")]
        public long AttributeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("attribute_name")]
        [JsonPropertyName("attribute_name")]
        public string AttributeName { get; set; }

        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Column("bus_cat_id")]
        [JsonPropertyName("bus_cat_id")]
        public int BusCatId { get; set; }
    }
}