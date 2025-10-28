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

        [Required]
        [Column("courier_service_name", TypeName = "varchar(255)")]
        public string CourierServiceName { get; set; }

        [Required]
        [Column("courier_contact_name", TypeName = "varchar(255)")]
        public string CourierContactName { get; set; }

        [StringLength(15)]
        [Column("courier_phone", TypeName = "varchar(15)")]
        public string CourierPhone { get; set; }

        [Required]
        [StringLength(100)]
        [Column("courier_email", TypeName = "varchar(100)")]
        public string CourierEmail { get; set; }

        [Column("registered_date")]
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        [Column("is_local")]
        public bool IsLocal { get; set; } = false;

        [Column("is_state")]
        public bool IsState { get; set; } = false;

        [Column("is_national")]
        public bool IsNational { get; set; } = false;

        [Column("is_international")]
        public bool IsInternational { get; set; } = false;

        // Navigation property
        public ICollection<CourierBranch> CourierBranches { get; set; }
    }
}
