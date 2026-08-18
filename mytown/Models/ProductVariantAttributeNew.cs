using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_variant_attributes")]
    public class ProductVariantAttributeNew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("variant_attribute_id")]
        public long VariantAttributeId { get; set; }

        [Column("variant_id")]
        public long VariantId { get; set; }

        [Column("attribute_id")]
        public long AttributeId { get; set; }

        [Column("attribute_value_id")]
        public long? AttributeValueId { get; set; }

        [Column("attribute_value")]
        public string? AttributeValue { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public virtual ProductVariantNew? Variant { get; set; }
    }
}