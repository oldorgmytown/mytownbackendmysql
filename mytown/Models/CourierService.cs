using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace mytown.Models
{
    [Table("courier_service")]
    public class CourierService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("courier_id")]
        public int CourierId { get; set; }

        // 🏢 Courier basic info
        [Required]
        [Column("courier_service_name", TypeName = "varchar(255)")]
        public string CourierServiceName { get; set; }

        [Required]
        [Column("courier_website_name", TypeName = "varchar(255)")]
        public string CourierWebsiteName { get; set; }

        [Required]
        [EmailAddress]
        [Column("courier_email", TypeName = "varchar(100)")]
        public string CourierEmail { get; set; }

        [Required]
        [StringLength(15)]
        [Column("courier_phone", TypeName = "varchar(15)")]
        public string CourierPhone { get; set; }

        // 🏠 Head Office Address
        [Required]
        [Column("address", TypeName = "varchar(255)")]
        public string Address { get; set; }

        [Required]
        [Column("town", TypeName = "varchar(100)")]
        public string Town { get; set; }

        [Required]
        [Column("city", TypeName = "varchar(100)")]
        public string City { get; set; }

        [Required]
        [Column("state", TypeName = "varchar(100)")]
        public string State { get; set; }

        [Required]
        [Column("country", TypeName = "varchar(100)")]
        public string Country { get; set; }

        [Required]
        [StringLength(10)]
        [Column("postal_code", TypeName = "varchar(10)")]
        public string PostalCode { get; set; }

        // 🚚 Service availability
        [Column("is_city")]
        public bool IsCity { get; set; }

        [Column("is_state")]
        public bool IsState { get; set; }

        // 🔐 Auth
        [Required]
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }

        // 📅 Meta
        [Column("registered_date")]
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;
   
        // Navigation property
        public ICollection<CourierBranch> CourierBranches { get; set; }

        public virtual ICollection<CourierVerification> CourierVerifications { get; set; }
    }
}
