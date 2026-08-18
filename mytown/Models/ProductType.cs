using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{

    [Table("product_type")]
    public class ProductType
    {
        [Key]
        [Column("prod_type_id")]
        [JsonPropertyName("prod_type_id")]
        public int ProdTypeId { get; set; }

        [Column("prod_subcat_id")]
        [JsonPropertyName("prod_subcat_id")]
        public int ProdSubcatId { get; set; }


        [Column("prod_group_id")]
        [JsonPropertyName("prod_group_id")]
        public int ProdGroupId { get; set; }

        [Column("prod_type_name")]
        [JsonPropertyName("prod_type_name")]
        public string ProdTypeName { get; set; }

    }
}
