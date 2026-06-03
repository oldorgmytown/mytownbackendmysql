using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("service_profiles")]
    public class ServiceProfile
    {
        [Key]
        [Column("service_profile_id")]
        public int ServiceProfileId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("bus_serv_id")]
        public int BusServId { get; set; }

       // [Required]
        [Column("business_name")]
        public string? BusinessName { get; set; }
       // [Required]
        [Column("business_location")]
        public string? BusinessLocation { get; set; }

        [Column("service_description")]
        public string ServiceDescription { get; set; } = string.Empty;

        [Column("years_of_experience")]
        public int? YearsOfExperience { get; set; }

        [StringLength(255)]
        [Column("govt_id_document")]
        public string? GovtIdDocument { get; set; }

        [StringLength(255)]
        [Column("professional_license")]
        public string? ProfessionalLicense { get; set; }

        [Column("service_available_locations")]
        public string? ServiceAvailableLocations { get; set; }

        [StringLength(100)]
        [Column("working_days")]
        public string? WorkingDays { get; set; }

        [Column("working_start_time")]
        public TimeSpan? WorkingStartTime { get; set; }

        [Column("working_end_time")]
        public TimeSpan? WorkingEndTime { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        [Column("service_logo")]
        public string? ServiceLogo { get; set; }

        [StringLength(255)]
        [Column("service_banner")]
        public string? ServiceBanner { get; set; }

        [StringLength(10)]
        [Column("status")]
        public string Status { get; set; }
    }
}