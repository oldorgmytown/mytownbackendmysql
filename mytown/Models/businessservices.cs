using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("business_services")]
    public class BusinessService
    {
        [Key]
        [Column("bus_serv_id")]
        public int BusServId { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        [Column("business_service_name")]
        public string BusinessServiceName { get; set; }
    }
}
