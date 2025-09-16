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

      

        // Navigation property
        public virtual product_sub_categories SubCategory { get; set; }
    }
}
