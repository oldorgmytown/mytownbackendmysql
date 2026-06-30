using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("guest_registers")]
    public class GuestRegister
    {
        [Key]
        [Column("guest_reg_id")]
        public int GuestRegId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("username")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Column("email")]
        public required string Email { get; set; }

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        // ✅ Password removed - guest checkout style

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

        [StringLength(200)]
        [Column("photo_name")]
        public string PhotoName { get; set; } = string.Empty;

        [Column("status")]
        public string? Status { get; set; }

        [Required]
        [Column("guest_reg_date")]
        public DateTime GuestRegDate { get; set; } = DateTime.UtcNow;
    }
}