using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("services")]
    public class Service
    {
        [Key]
        [Column("service_id")]
        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [Column("bus_reg_id")]
        [JsonPropertyName("bus_reg_id")]
        public int BusRegId { get; set; }

        [Column("bus_serv_id")]
        [JsonPropertyName("bus_serv_id")]
        public int BusServId { get; set; }

        [Column("serv_subcat_id")]
        [JsonPropertyName("serv_subcat_id")]
        public int ServSubcatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("service_name")]
        [JsonPropertyName("service_name")]
        public string ServiceName { get; set; }

        [Column("service_subject")]
        [JsonPropertyName("service_subject")]
        public string ServiceSubject { get; set; }

        [Column("service_description")]
        [JsonPropertyName("service_description")]
        public string ServiceDescription { get; set; }

        [Column("service_image")]
        [JsonPropertyName("service_image")]
        public string ServiceImage { get; set; }

        [Range(0, double.MaxValue)]
        [Column("service_cost", TypeName = "decimal(10,2)")]
        [JsonPropertyName("service_cost")]
        public decimal ServiceCost { get; set; }
    }
}
