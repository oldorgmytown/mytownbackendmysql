using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_db_notifications")]
    public class SenderDBNotifications
    {
        [Key]
        [DatabaseGenerated(
            DatabaseGeneratedOption.Identity)]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("sender_reg_id")]
        public int SenderRegId { get; set; }

        [ForeignKey(nameof(SenderRegId))]
        public SenderRegister SenderRegister { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }
            = false;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;
    }
}