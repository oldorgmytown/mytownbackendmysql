using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
   
        [Table("designs")]
        public class Design
        {
            [Key]
            [Column("design_id")]
            [JsonPropertyName("design_id")]
            public int DesignId { get; set; }

            [Column("prod_subcat_id")]
            [JsonPropertyName("prod_subcat_id")]
            public int ProdSubcatId { get; set; }

            [Required]
            [StringLength(100)]
            [Column("design_name")]
            [JsonPropertyName("design_name")]
            public string DesignName { get; set; } = string.Empty;
        }
    
}
