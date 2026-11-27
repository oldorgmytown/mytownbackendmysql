using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    /// <summary>
    /// Dates / time ranges the provider has blocked (leave, holiday, busy).
    /// StartTime and EndTime both NULL = the whole day is blocked.
    /// </summary>
    [Table("service_blocked_slots")]
    public class ServiceBlockedSlot
    {
        [Key]
        [Column("blocked_slot_id")]
        public int BlockedSlotId { get; set; }

        [Required]
        [Column("service_profile_id")]
        public int ServiceProfileId { get; set; }

        [Column("enquiry_id")]
        public int? EnquiryId { get; set; }

        [Required]
        [Column("blocked_date")]
        public DateTime BlockedDate { get; set; }

        [Column("start_time")]
        public TimeSpan? StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan? EndTime { get; set; }

        [StringLength(255)]
        [Column("reason")]
        public string? Reason { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
