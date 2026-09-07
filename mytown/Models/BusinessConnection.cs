using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("business_connections")]
    public class BusinessConnection
    {
        [Key]
        [Column("id")]
        public int BusConnectionId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required]
        [Column("connected_on")]
        public DateTime ConnectedOn { get; set; } = DateTime.UtcNow;

        [Column("status")]
        public bool Status { get; set; } = true;

        [ForeignKey(nameof(BusRegId))]
        public virtual BusinessRegister BusinessRegister { get; set; }

        [ForeignKey(nameof(ShopperRegId))]
        public virtual ShopperRegister ShopperRegister { get; set; }
    }
}