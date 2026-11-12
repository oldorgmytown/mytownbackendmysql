using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_registers")]
    public class TransporterRegister
    {
        [Key]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("transporter_name")]
        public required string TransporterName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Column("transporter_email")]
        public required string Email { get; set; }

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Column("password")]
        public required string Password { get; set; }

        [Required]
        [StringLength(300)]
        [Column("address")]
        public required string Address { get; set; }

        [Required]
        [StringLength(100)]
        [Column("town")]
        public required string Town { get; set; }

        [Required]
        [StringLength(100)]
        [Column("city")]
        public required string City { get; set; }

        [Required]
        [StringLength(100)]
        [Column("state")]
        public required string State { get; set; }

        [Required]
        [StringLength(100)]
        [Column("country")]
        public required string Country { get; set; }

        [StringLength(10)]
        [Column("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        [Column("phone_number")]
        public required string PhoneNumber { get; set; }

        //[StringLength(200)]
        //[Column("photo_name")]
        //public string PhotoName { get; set; } = string.Empty;

        [Column("status")]
        public string? Status { get; set; } = "Pending"; // Default status when a transporter registers

        [Required]
        [Column("transporter_reg_date")]
        public DateTime TransporeterRegDate { get; set; } = DateTime.UtcNow;
    }
}
