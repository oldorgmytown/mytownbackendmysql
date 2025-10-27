using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("business_categories")]
    public class BusinessCategory
    {
        [Key]
        [Column("bus_cat_id")]
        public int BusCatId { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        [Column("business_category_name")]
        public string BusinessCategoryName { get; set; }
    }
}
