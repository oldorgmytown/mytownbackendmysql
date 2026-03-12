using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("store_orders")]
    public class StoreOrder
    {
        [Key]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        [Column("store_id")]
        public int StoreId { get; set; }
        public BusinessRegister Store { get; set; }

        [Column("courier_type")]
        public string CourierType { get; set; }   // e.g., Fast, Regular, etc.

        [Column("store_total_amount")]
        public decimal StoreTotalAmount { get; set; }

        [Column("status")]
        public string Storeorder_Status { get; set; }        // Delivery level -  Pending, Ready to ship, In progress, Delivered

        public ICollection<orderdetails> OrderDetails { get; set; }
    }
}

