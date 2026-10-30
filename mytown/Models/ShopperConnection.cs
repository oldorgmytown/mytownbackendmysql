using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shopper_connections")]
    public class ShopperConnection
    {
        [Key]
        [Column("connection_id")]
        public int ConnectionId { get; set; }

        [Column("requester_shopper_id")]
        public int RequesterShopperId { get; set; }

        [Column("receiver_shopper_id")]
        public int ReceiverShopperId { get; set; }

        [Column("status")]
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Pending;

        [Column("requested_at")]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }

        [ForeignKey(nameof(RequesterShopperId))]
        public ShopperRegister? Requester { get; set; }

        [ForeignKey(nameof(ReceiverShopperId))]
        public ShopperRegister? Receiver { get; set; }
    }
}
