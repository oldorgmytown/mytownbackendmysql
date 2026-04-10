using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class PendingSenderVerification
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

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("json_payload")]
        public string JsonPayload { get; set; }
    }
}
