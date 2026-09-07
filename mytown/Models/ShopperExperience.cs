using mytown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("shopper_experiences")]
    public class ShopperExperience
    {
        [Key]
        [Column("shopper_experience_id")]
        public int ShopperExperienceId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("post_type")]
        public string PostType { get; set; }

        [Column("rating")]
        public decimal? Rating { get; set; }

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("experience")]
        public string Experience { get; set; }


        [Column("status")]
        public string Status { get; set; } = "Approved";



        [Column("is_anonymous")]
        public bool IsAnonymous { get; set; } = false;

        [Column("verified_purchase")]
        public bool VerifiedPurchase { get; set; } = false;

       

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ShopperRegId))]
        public virtual ShopperRegister ShopperRegister { get; set; }

        [ForeignKey(nameof(BusRegId))]
        public virtual BusinessRegister BusinessRegister { get; set; }
        public virtual ICollection<ShopperExperiencePhoto> Photos { get; set; }
       = new List<ShopperExperiencePhoto>();
    }
}