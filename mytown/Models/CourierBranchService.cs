using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_branch_service")]
    public class CourierBranchService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("branch_service_id")]
        public int BranchServiceId { get; set; }

        [Required]
        [ForeignKey(nameof(CourierBranch))]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [Column("destinations", TypeName = "varchar(255)")]
        public string Destinations { get; set; }

        [Required]
        [Column("shipping_mode", TypeName = "varchar(50)")]
        public string ShippingMode { get; set; }

        [Column("distance_range", TypeName = "varchar(100)")]
        public string DistanceRange { get; set; }

        [Column("weight_range", TypeName = "varchar(100)")]
        public string WeightRange { get; set; }

        [Column("charges", TypeName = "decimal(10,2)")]
        public decimal Charges { get; set; }

        [Column("estimate_days")]
        public int? EstimateDays { get; set; }

        // 🔗 Navigation
        public CourierBranch CourierBranch { get; set; }
    }
}
