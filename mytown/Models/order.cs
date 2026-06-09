using mytown.Models.DTO_s;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

       // [Required]
        [Column("shopper_reg_id")]
        public int? ShopperRegId { get; set; }

        [ForeignKey("ShopperRegId")]
        public ShopperRegister ShopperRegister { get; set; }

        [Column("guest_reg_id")]
        public int? GuestRegId { get; set; }

        [ForeignKey("GuestRegId")]
        public GuestRegister? GuestRegister { get; set; }

        [Column("is_guest_order")]
        public bool IsGuestOrder { get; set; }

       
        // Selected delivery address
        [Column("selected_alt_address_id")]
        public int? SelectedAltAddressId { get; set; }

        // Optional navigation (recommended)
        [ForeignKey("SelectedAltAddressId")]
        public ShopperAlternateAddress? SelectedAlternateAddress { get; set; }

        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("shipping_type")]
        public string ShippingType { get; set; }

        [Column("orderstatus")]
        public string OrderStatus { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        public virtual ICollection<orderdetails> OrderDetails { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<ShippingDetails> ShippingDetails { get; set; }
    }

}
