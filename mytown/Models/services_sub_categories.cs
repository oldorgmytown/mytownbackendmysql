using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("services_sub_categories")]
    public class ServiceSubCategory
    {
        [Key]
        [Column("serv_subcat_id")]
        public int ServSubcatId { get; set; }

        [Required]
        [Column("bus_serv_id")]
        public int BusServId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("service_type_name")]
        public string ServiceTypeName { get; set; }
    }
}