using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("courier_branch")]
    public class CourierBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(CourierService))]
        [Column("courier_id")]
        public int CourierId { get; set; }

        [Column("courier_name", TypeName = "varchar(255)")]
        public string CourierName { get; set; } // Optional: for display purposes only

        [Required]
        [Column("country", TypeName = "varchar(100)")]
        public string Country { get; set; }

        [Required]
        [Column("state", TypeName = "varchar(100)")]
        public string State { get; set; }

        [Required]
        [Column("city", TypeName = "varchar(100)")]
        public string City { get; set; }

        [Column("town", TypeName = "varchar(100)")]
        public string Town { get; set; }

        [Column("branch_address", TypeName = "varchar(255)")]
        public string BranchAddress { get; set; }

        [Column("branch_phone_number", TypeName = "varchar(20)")]
        public string BranchPhoneNumber { get; set; }

        [Column("branch_email_id", TypeName = "varchar(100)")]
        public string BranchEmailId { get; set; }

        [Column("branch_contact_person", TypeName = "varchar(100)")]
        public string BranchContactPerson { get; set; }

        [Column("destinations", TypeName = "varchar(255)")]
        public string Destinations { get; set; } // To be normalized later

        [Required]
        [Column("shipping_mode", TypeName = "varchar(50)")]
        public string ShippingMode { get; set; } // Air / Surface

        [Column("charges", TypeName = "decimal(10,2)")]
        public decimal Charges { get; set; }

        [Column("weight_range", TypeName = "varchar(100)")]
        public string WeightRange { get; set; }

        [Column("distance_range", TypeName = "varchar(100)")]
        public string DistanceRange { get; set; }

        // 🔗 Navigation property
        public CourierService CourierService { get; set; }
    }
}

