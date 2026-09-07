using mytown.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTown.Models
{
    [Table("business_registers")]
    public class BusinessRegister
    {
        [Key]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("business_username")]
        public string BusinessUsername { get; set; }

        [Required]
        [StringLength(100)]
        [Column("business_name")]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("license_type")]
        public string LicenseType { get; set; }

        [StringLength(15)]
        [Column("gstin")]
        public string Gstin { get; set; }

        [Required]
        [Column("bus_serv_id")]
        public int BusServId { get; set; }

        [Required]
        [Column("bus_cat_id")]
        public int BusCatId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("town")]
        public string Town { get; set; }

        [Required]
        [StringLength(15)]
        [Column("bus_mobile_no")]
        public string BusMobileNo { get; set; }

        [Required]
        [StringLength(100)]
        [Column("bus_email")]
        public string BusEmail { get; set; }

        [Column("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        [Column("currency")]
        [StringLength(10)]
        public string? Currency { get; set; }

        [Required]
        [StringLength(200)]
        [Column("address_1")]
        public string Address1 { get; set; }

        [StringLength(200)]
        [Column("address_2")]
        public string? Address2 { get; set; }

        [Required]
        [StringLength(100)]
        [Column("business_city")]
        public string BusinessCity { get; set; }

        [Required]
        [StringLength(100)]
        [Column("business_state")]
        public string BusinessState { get; set; }

        [Required]
        [StringLength(100)]
        [Column("business_country")]
        public string BusinessCountry { get; set; }

        [StringLength(10)]
        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Column("password")]
        public string Password { get; set; }

        [Column("business_reg_date")]
        public DateTime BusinessRegDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual BusinessProfile? BusinessProfile { get; set; }

        [ForeignKey(nameof(BusCatId))]
        public virtual BusinessCategory BusinessCategory { get; set; }
        public virtual BusinessAccountDetail? BusinessAccountDetail { get; set; }
    }
}
