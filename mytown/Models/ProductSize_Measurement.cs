using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace mytown.Models
{
  
        [Table("productsize_measurements")]
        public class ProductSize_Measurement
        {
            [Key]
            [Column("measurement_id")]
            public int MeasurementId { get; set; }

            [Required]
            [Column("size_id")]
            public int SizeId { get; set; }

            [Column("length", TypeName = "decimal(10,2)")]
            public decimal? Length { get; set; }

            [Column("height", TypeName = "decimal(10,2)")]
            public decimal? Height { get; set; }

            [Column("width", TypeName = "decimal(10,2)")]
            public decimal? Width { get; set; }

            [Column("weight", TypeName = "decimal(10,2)")]
            public decimal? Weight { get; set; }

            [Column("unit")]
            [StringLength(20)]
            public string? Unit { get; set; }

            // Navigation property (link back to Sizes table)
            [ForeignKey("SizeId")]
            public virtual Product_Sizes? Size { get; set; }
        }
    
}
