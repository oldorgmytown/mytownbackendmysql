using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//latest table
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

        [Column("courier_service_name", TypeName = "varchar(255)")]
        public string CourierServiceName { get; set; } // Optional: for display purposes only

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

        [Column("address", TypeName = "varchar(255)")]
        public string BranchAddress { get; set; }

        [Column("branch_phone_number", TypeName = "varchar(20)")]
        public string BranchPhoneNumber { get; set; }

        [Column("branch_email_id", TypeName = "varchar(100)")]
        public string BranchEmailId { get; set; }

        [Column("branch_contact_person", TypeName = "varchar(100)")]
        public string BranchContactPerson { get; set; }


        // 🔐 Branch login (for future)
        [Column("password_hash", TypeName = "varchar(255)")]
        public string PasswordHash { get; set; } = "Branch@123";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        //[Column("destinations", TypeName = "varchar(255)")]
        //public string Destinations { get; set; } // To be normalized later

        //[Required]
        //[Column("shipping_mode", TypeName = "varchar(50)")]
        //public string ShippingMode { get; set; } // Air / Surface

        //[Column("charges", TypeName = "decimal(10,2)")]
        //public decimal Charges { get; set; }

        //[Column("weight_range", TypeName = "varchar(100)")]
        //public string WeightRange { get; set; }

        //[Column("distance_range", TypeName = "varchar(100)")]
        //public string DistanceRange { get; set; }

        //[Column("estimate_days", TypeName = "int")]
        //public int? EstimateDays { get; set; }   // nullable, because CSV may not have value


        // 🔗 Navigation property
        public CourierService CourierService { get; set; }
        public ICollection<CourierBranchService> Services { get; set; }
    }
}

