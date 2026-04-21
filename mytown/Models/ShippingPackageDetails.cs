using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("shipping_package_details")]
    public class ShippingPackageDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("package_detail_id")]
        public int PackageDetailId { get; set; }

        [Column("shipping_detail_id")]
        public int ShippingDetailId { get; set; }

        [Column("store_order_id")]
        public int StoreOrderId { get; set; }

        [Column("package_length", TypeName = "decimal(10,2)")]
        public decimal? PackageLength { get; set; }

        [Column("package_width", TypeName = "decimal(10,2)")]
        public decimal? PackageWidth { get; set; }

        [Column("package_height", TypeName = "decimal(10,2)")]
        public decimal? PackageHeight { get; set; }

        [Column("package_weight", TypeName = "decimal(10,2)")]
        public decimal? PackageWeight { get; set; }

        [Column("dimension_unit")]
        public string DimensionUnit { get; set; }

        [Column("weight_unit")]
        public string WeightUnit { get; set; }
    }
}