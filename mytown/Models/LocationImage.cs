using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("location_images")]
    public class LocationImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("country", TypeName = "varchar(100)")]
        public string Country { get; set; }

        [Column("state_name", TypeName = "varchar(100)")]
        public string? StateName { get; set; }

        [Column("city", TypeName = "varchar(100)")]
        public string? City { get; set; }

        [Required]
        [Column("image", TypeName = "varchar(255)")]
        public string Image { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}