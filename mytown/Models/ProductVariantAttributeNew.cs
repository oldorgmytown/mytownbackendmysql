using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_variant_attributes")]
    public class ProductVariantAttributeNew
    {
        [Key]
        [Column("variant_attribute_id")]
        [JsonPropertyName("variant_attribute_id")]
        public long VariantAttributeId { get; set; }

        [ForeignKey(nameof(Variant))]
        [Column("variant_id")]
        [JsonPropertyName("variant_id")]
        public long VariantId { get; set; }

        [Column("attribute_id")]
        [JsonPropertyName("attribute_id")]
        public long AttributeId { get; set; }

        [Column("attribute_value_id")]
        [JsonPropertyName("attribute_value_id")]
        public long AttributeValueId { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Parent Variant

        [JsonIgnore]
        public virtual ProductVariantNew? Variant { get; set; }
    }
}