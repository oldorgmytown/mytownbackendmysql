using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("pending_guest_verification")]
    public class PendingGuestVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

       
        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("json_payload")]
        public string JsonPayload { get; set; }
    }
}