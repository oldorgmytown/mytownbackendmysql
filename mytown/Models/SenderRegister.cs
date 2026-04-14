using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_registers")]
    public class SenderRegister
    {
        [Key]
        [Column("sender_reg_id")]
        public int SenderRegId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("sender_name")]
        public required string SenderName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Column("sender_email")]
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

        [Column("status")]
        public string? Status { get; set; }

        [Required]
        [Column("sender_reg_date")]
        public DateTime SenderRegDate { get; set; } = DateTime.UtcNow;
    }
}