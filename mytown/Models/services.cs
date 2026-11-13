using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("services")]
    public class Service
    {
        [Key]
        [Column("service_id")]
        public int ServiceId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("bus_serv_id")]
        public int BusServId { get; set; }

        [Required]
        [Column("serv_subcat_id")]
        public int ServSubcatId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("service_name")]
        public string ServiceName { get; set; }

        [Column("service_type_description")]
        public string? ServiceTypeDescription { get; set; }

        [Column("inspection_fee")]
        public decimal? InspectionFee { get; set; }

        [Column("starting_price")]
        public decimal? StartingPrice { get; set; }

        [StringLength(100)]
        [Column("estimated_duration")]
        public string? EstimatedDuration { get; set; }

        [StringLength(200)]
        [Column("service_type_image")]
        public string? ServiceTypeImage { get; set; }


    }
}