using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("business_db_notifications")]
    public class BusinessDBNotifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        // Business store
        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
