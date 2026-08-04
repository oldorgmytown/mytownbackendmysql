using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mytown.Enums;


namespace MyTown.Models
{
    [Table("business_account_details")]
    public class BusinessAccountDetail
    {
        [Key]
        [Column("account_detail_id")]
        public int AccountDetailId { get; set; }

        [Required]
        [Column("bus_reg_id")]
        public int BusRegId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("account_holder_name")]
        public string AccountHolderName { get; set; }

        [Required]
        [StringLength(150)]
        [Column("bank_name")]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(20)]
        [Column("ifsc_code")]
        public string IFSCCode { get; set; }

        #region Bank Verification

        [StringLength(20)]
        [Column("bank_verification_status")]
        public BankVerificationStatus BankVerificationStatus { get; set; }
    = BankVerificationStatus.Pending;

        [Column("bank_verified_date")]
        public DateTime? BankVerifiedDate { get; set; }

        [StringLength(100)]
        [Column("bank_verification_reference")]
        public string? BankVerificationReference { get; set; }

        [StringLength(500)]
        [Column("bank_verification_message")]
        public string? BankVerificationMessage { get; set; }

        #endregion

        #region Razorpay Contact

        [StringLength(100)]
        [Column("razorpay_contact_id")]
        public string? RazorpayContactId { get; set; }

        [StringLength(20)]
        [Column("razorpay_contact_status")]
        public RazorpayContactStatus RazorpayContactStatus { get; set; }
    = RazorpayContactStatus.NotCreated;

       

        [Column("contact_created_date")]
        public DateTime? ContactCreatedDate { get; set; }

        #endregion

        #region Razorpay Fund Account

        [StringLength(100)]
        [Column("razorpay_fund_account_id")]
        public string? RazorpayFundAccountId { get; set; }

        [StringLength(20)]
        [Column("razorpay_fund_account_status")]
        public RazorpayFundAccountStatus RazorpayFundAccountStatus { get; set; }
            = RazorpayFundAccountStatus.NotCreated;

        [Column("fund_account_created_date")]
        public DateTime? FundAccountCreatedDate { get; set; }

        #endregion

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        [Column("remarks")]
        public string? Remarks { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(BusRegId))]
        public virtual BusinessRegister BusinessRegister { get; set; }
    }
}