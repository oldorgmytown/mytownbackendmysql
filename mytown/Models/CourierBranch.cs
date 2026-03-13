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

       

        // 🔗 Navigation property
        public CourierService CourierService { get; set; }
        public ICollection<CourierBranchService> Services { get; set; }
    }
}

