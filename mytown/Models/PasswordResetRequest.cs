using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("password_reset_requests")]
    public class PasswordResetRequest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("token")]
        public string Token { get; set; }

        [Required]
        [Column("expiry")]
        public DateTime Expiry { get; set; }
    }
}
