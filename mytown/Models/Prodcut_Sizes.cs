using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("product_sizes")]
    public class Product_Sizes
    {
        [Key]
        [Column("size_id")]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SubCategory))]
        [Column("prod_subcat_id")]
        public int prod_subcat_id { get; set; }  

        [Required]
        [StringLength(100)]
        [Column("size_name")]
        public string SizeName { get; set; }


        // ✅ New fields for dimensions
        [Column("length")]
        public decimal? Length { get; set; }

        [Column("width")]
        public decimal? Width { get; set; }

        [Column("height")]
        public decimal? Height { get; set; }

        [Column("weight")]
        public decimal? Weight { get; set; }

        // Navigation property
        public virtual ProductSubCategory SubCategory { get; set; }
    }
}
