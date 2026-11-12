using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
   
        [Table("shopper_experience_photos")]
        public class ShopperExperiencePhoto
        {
            [Key]
            [Column("shopper_experience_photo_id")]
            public int ShopperExperiencePhotoId { get; set; }

            [Column("shopper_experience_id")]
            public int ShopperExperienceId { get; set; }

            [Column("photo_url")]
            public string PhotoUrl { get; set; }

            [Column("created_date")]
            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

            // Navigation property
            [ForeignKey(nameof(ShopperExperienceId))]
            public virtual ShopperExperience ShopperExperience { get; set; }
        }
    }

