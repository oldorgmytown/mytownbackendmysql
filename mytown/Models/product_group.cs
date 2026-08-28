using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_group")]
    public class ProductGroup
    {
        [Key]
        [Column("product_group_id")]
        [JsonPropertyName("prod_group_id")]
        public int ProdGroupId { get; set; }

        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("product_group_name")]
        [JsonPropertyName("prod_group_name")]
        public string ProdGroupName { get; set; }

    }
}