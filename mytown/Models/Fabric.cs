using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("fabrics")]
    public class Fabric
    {
        [Key]
        [Column("fabric_id")]
        [JsonProperty("fabric_id")]
        public int FabricId { get; set; }

        [Column("prod_subcat_id")]
        [JsonProperty("prod_subcat_id")]
        public int ProdSubcatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("fabric_name")]
        [JsonProperty("fabric_name")]
        public string FabricName { get; set; }
    }



}
