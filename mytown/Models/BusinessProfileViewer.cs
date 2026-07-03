using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
     [Table("business_profile_viewers")]
        public class BusinessProfileViewer
        {
            [Key]
            [Column("id")]
            public int ProfileViewId { get; set; }

            [Required]
            [Column("bus_reg_id")]
            public int BusRegId { get; set; }

            [Required]
            [Column("shopper_reg_id")]
            public int ShopperRegId { get; set; }

            [Required]
            [Column("last_seen")]
            public DateTime LastSeen { get; set; }

            [ForeignKey(nameof(BusRegId))]
            public virtual BusinessRegister Business { get; set; }

            [ForeignKey(nameof(ShopperRegId))]
            public virtual ShopperRegister Shopper { get; set; }
        }
    
}
