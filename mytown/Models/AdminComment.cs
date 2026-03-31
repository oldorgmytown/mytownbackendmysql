using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("admin_comments")]
    public class AdminComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [ForeignKey(nameof(BusRegId))]
        public BusinessRegister BusinessRegister { get; set; }

        [Column("comments")]
        [MaxLength(500)]
        public string Comments { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}



