using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models.DTO_s
{
    [Table("transporter_db_notifications")]
    public class TransporterDBNotifications
    {
   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }        

        // Notification content
        [Required]
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }

        [Required]
        [Column("message", TypeName = "text")]
        public string Message { get; set; }

        // Read status
        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        // Timestamp
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
