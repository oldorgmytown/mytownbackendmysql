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
            [Column("CommentId")]
            public int CommentId { get; set; }

            [Required]
            [Column("BusRegId")]
            public int BusRegId { get; set; }

            [ForeignKey(nameof(BusRegId))]
            public BusinessRegister BusinessRegister { get; set; }

            [Column("Comments")]
            [MaxLength(500)]
            public string Comments { get; set; }

            [Column("Status")]
            [MaxLength(50)]
            public string Status { get; set; }

            [Column("CreatedAt")]
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            [Column("UpdatedAt")]
            public DateTime UpdatedAt { get; set; } = DateTime.Now;
        }
    }



