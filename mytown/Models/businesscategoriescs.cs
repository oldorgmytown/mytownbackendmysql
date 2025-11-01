using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("business_categories")]
    public class BusinessCategory
    {
        [Key]
        [Column("bus_cat_id")]
        [JsonPropertyName("buscatId")]
        public int BusCatId { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        [Column("business_category_name")]
        [JsonPropertyName("businesscategory_name")]
        public string BusinessCategoryName { get; set; }
    }
}
