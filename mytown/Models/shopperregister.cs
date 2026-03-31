using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shopper_registers")]
    public class ShopperRegister
    {
        [Key]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

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

        [StringLength(200)]
        [Column("photo_name")]
        public string PhotoName { get; set; } = string.Empty;

        [Column("status")]
        public string? Status { get; set; }

        [Required]
        [Column("shopper_reg_date")]
        public DateTime ShopperRegDate { get; set; } = DateTime.UtcNow;
    }
}
