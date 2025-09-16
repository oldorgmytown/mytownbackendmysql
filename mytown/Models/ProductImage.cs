using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class ProductImage
    {

        [Key]
        public int ImageId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductVariant))]
        [Column("sku_id")]
        public int? SkuId { get; set; }   

        [Required, StringLength(500)]
        public string FileName { get; set; }

        public int SortOrder { get; set; }

        public virtual products Product { get; set; }
        public virtual Sku_ProductVariant? ProductVariant { get; set; }
    }
        

    }
