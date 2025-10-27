
using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class orderdetails
    {
        [Key]
        public int OrderDetailId { get; set; } // Primary Key

        [Required]
        public int OrderId { get; set; } // Foreign Key - Orders

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        public int ProductId { get; set; } // Foreign Key - Products
       

        public products Product { get; set; }
        [Required]
        public int sku_id { get; set; } // Foreign Key - varinats

        public Sku_ProductVariant Variant { get; set; }

        [Required]
        public int StoreId { get; set; } // Foreign Key - Store

        public BusinessRegister Store { get; set; }

        [Required]
        public int Quantity { get; set; } // Quantity Ordered

        [Required]
        public decimal Price { get; set; } // Price at Purchase
    }

}
