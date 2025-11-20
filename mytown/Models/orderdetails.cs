
using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("orderdetails")]
    public class orderdetails
    {
        [Key]
        [Column("order_detail_id")]
        public int OrderDetailId { get; set; } // Primary Key

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; } // Foreign Key - Orders

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [ForeignKey("StoreOrderId")]
        public StoreOrder StoreOrder { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; } // Foreign Key - Products

        public Products Product { get; set; }

        [Required]
        [Column("sku_id")]
        [JsonPropertyName("sku_id")]
        public int SkuId { get; set; }

        public Sku_ProductVariant Variant { get; set; }

        [Required]
        [Column("store_id")]
        public int StoreId { get; set; } // Foreign Key - Store

        public BusinessRegister Store { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } // Quantity Ordered

        [Required]
        [Column("price")]
        public decimal Price { get; set; } // Price at Purchase
    }

}
