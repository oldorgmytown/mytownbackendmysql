using MyTown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    public class products
    {
        [Column("product_id")]
        [Key]
        public int product_id {  get; set; }

        [ForeignKey("BusinessRegister")]
        public int BusRegId { get; set; }

        public int BuscatId { get; set; }

        public int prod_subcat_id { get; set; }

        [Required]
        [StringLength(100)]
        public string? product_name { get; set; }

        
        [StringLength(100)]
        public string? product_subject { get; set; }

        [Required]
        [StringLength(500)]
        public string? product_description   { get; set; }
        public string? product_image { get; set; }
        public string? supplier_name { get; set; }

        public int? ProductTypeId { get; set; }
        public int? FabricId { get; set; }
        public int? DesignId { get; set; }

        // --- NEW relationships you want on the Product itself ---
        [ForeignKey("ProductTypeId")]
        public virtual ProductType? ProductType { get; set; }

        [ForeignKey("FabricId")]
        public virtual Fabric? Fabric { get; set; }

        [ForeignKey("DesignId")]
        public virtual Design? Design { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? product_cost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? product_length { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? product_width { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? product_weight { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? product_quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? product_height { get; set; }

        [Range(0, 100)]
        public decimal? discount { get; set; }   // nullable

        [Range(0, double.MaxValue)]
        public decimal? discount_price { get; set; }  // nullable

        public string? color { get; set; }

        public string? size { get; set; }

        [JsonIgnore]
        public virtual BusinessRegister? BusinessRegister { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<Sku_ProductVariant> Sku_ProductVariants { get; set; }
        = new List<Sku_ProductVariant>();


    }
}
