using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_service")]
    public class CourierService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [Required]
        [Column("courier_service_name", TypeName = "varchar(255)")]
        public string CourierServiceName { get; set; } = null!;

        [Required]
        [Column("courier_website_name", TypeName = "varchar(255)")]
        public string CourierWebsiteName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Column("courier_email", TypeName = "varchar(100)")]
        public string CourierEmail { get; set; } = null!;

        [Required]
        [StringLength(15)]
        [Column("courier_phone", TypeName = "varchar(15)")]
        public string CourierPhone { get; set; } = null!;

        [Required]
        [Column("address", TypeName = "varchar(255)")]
        public string Address { get; set; } = null!;

        [Required]
        [Column("town", TypeName = "varchar(100)")]
        public string Town { get; set; } = null!;

        [Required]
        [Column("city", TypeName = "varchar(100)")]
        public string City { get; set; } = null!;

        [Required]
        [Column("state", TypeName = "varchar(100)")]
        public string State { get; set; } = null!;

        [Required]
        [Column("country", TypeName = "varchar(100)")]
        public string Country { get; set; } = null!;

        [Required]
        [StringLength(10)]
        [Column("postal_code", TypeName = "varchar(10)")]
        public string PostalCode { get; set; } = null!;

        [Column("is_city")]
        public bool IsCity { get; set; }

        [Column("is_state")]
        public bool IsState { get; set; }

        [Required]
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; } = null!;

        [Column("registered_date")]
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        [Column("profile_status")]
        public string ProfileStatus { get; set; } = "Incomplete";

        public ICollection<CourierBranch> CourierBranches { get; set; } = new List<CourierBranch>();

        public virtual ICollection<CourierVerification> CourierVerifications { get; set; } = new List<CourierVerification>();
    }
}