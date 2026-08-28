using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_attribute_values")]
    public class ProductAttributeValue
    {
        [Key]
        [Column("attribute_value_id")]
        [JsonPropertyName("attribute_value_id")]
        public int AttributeValueId { get; set; }

        [Column("attribute_id")]
        [JsonPropertyName("attribute_id")]
        public int AttributeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("attribute_value")]
        [JsonPropertyName("attribute_value")]
        public string AttributeValue { get; set; }
    }
}