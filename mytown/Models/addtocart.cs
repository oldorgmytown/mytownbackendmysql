using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class addtocart
    {
        [Key]
        public int CartId { get; set; } // Primary key
        [Required]
        public int product_id { get; set; }
        [Required]
        public int sku_id { get; set; }
        [Required]
        public int BusRegId { get; set; }
        [Required]
        public int BuscatId { get; set; }
        [Required]
        public int prod_subcat_id { get; set; }
        [Required]
        public int ShopperRegId { get; set; }
        public int prod_qty { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal product_price { get; set; }
        public string orderstatus { get; set; }



    }
}
