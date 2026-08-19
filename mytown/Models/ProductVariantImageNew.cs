using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("product_variant_images")]
    public class ProductVariantImageNew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_id")]
        [JsonPropertyName("image_id")]
        public long ImageId { get; set; }

        [Column("variant_id")]
        [JsonPropertyName("variant_id")]
        public long VariantId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("file_name")]
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Column("sort_order")]
        [JsonPropertyName("sort_order")]
        public int SortOrder { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public virtual ProductVariantNew? ProductVariant { get; set; }
    }
}