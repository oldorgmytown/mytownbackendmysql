using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    [Table("courier_service")]
    public class CourierService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("courier_id")]
        public int CourierId { get; set; }

        // Basic Details
        [Required]
        [Column("courier_service_name", TypeName = "varchar(255)")]
        public string CourierServiceName { get; set; }

        [Column("courier_website", TypeName = "varchar(255)")]
        public string CourierWebsite { get; set; }

        // Address Details
        [Column("head_office_address", TypeName = "varchar(500)")]
        public string HeadOfficeAddress { get; set; }

        [Column("town", TypeName = "varchar(150)")]
        public string Town { get; set; }

        [Column("city", TypeName = "varchar(150)")]
        public string City { get; set; }

        [Column("state", TypeName = "varchar(150)")]
        public string State { get; set; }

        [Column("country", TypeName = "varchar(150)")]
        public string Country { get; set; }

        [Column("postal_code", TypeName = "varchar(20)")]
        public string PostalCode { get; set; }

        // Contact
        [StringLength(15)]
        [Column("courier_phone", TypeName = "varchar(15)")]
        public string CourierPhone { get; set; }

        [Required]
        [StringLength(100)]
        [Column("courier_email", TypeName = "varchar(100)")]
        public string CourierEmail { get; set; }

        // Capability Flags
        [Column("is_city")]
        public bool IsCity { get; set; } = false;

        [Column("is_state")]
        public bool IsState { get; set; } = false;

        [Column("is_country")]
        public bool IsCountry { get; set; } = false;

        [Column("is_international")]
        public bool IsInternational { get; set; } = false;

        // Login
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        // Other
        [Column("registered_date")]
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<CourierBranch> CourierBranches { get; set; }
    }
}
