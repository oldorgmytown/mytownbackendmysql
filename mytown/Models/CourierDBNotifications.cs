using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_db_notifications")]
    public class CourierDBNotifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        // 🔗 Courier (parent)
        [Required]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public CourierService CourierService { get; set; }

        // 🔗 Branch (optional but recommended)
        [Required]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public CourierBranch CourierBranch { get; set; }

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
