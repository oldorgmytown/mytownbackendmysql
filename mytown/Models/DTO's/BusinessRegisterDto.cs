using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class BusinessRegisterDto
    {
        public int BusRegId { get; set; }

        [Required(ErrorMessage = "Business username is required.")]
        [StringLength(100, ErrorMessage = "Business username cannot exceed 100 characters.")]
        public string BusinessUsername { get; set; }

        [Required(ErrorMessage = "Business name is required.")]
        [StringLength(150, ErrorMessage = "Business name cannot exceed 150 characters.")]
        public string Businessname { get; set; }

        [Required(ErrorMessage = "License type is required.")]
        public string LicenseType { get; set; }

        [Required(ErrorMessage = "GSTIN is required.")]
        [StringLength(20, ErrorMessage = "GSTIN cannot exceed 20 characters.")]
        public string Gstin { get; set; }

        [Required(ErrorMessage = "Business service ID is required.")]
        public int BusservId { get; set; }

        [Required(ErrorMessage = "Business category ID is required.")]
        public int BuscatId { get; set; }

        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid mobile number format.")]
        public string BusMobileNo { get; set; }

        [Required(ErrorMessage = "Business email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string BusEmail { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required.")]
        public string Address1 { get; set; }

        public string Address2 { get; set; } // optional

        [Required(ErrorMessage = "Business city is required.")]
        public string businessCity { get; set; }

        [Required(ErrorMessage = "Business state is required.")]
        public string businessState { get; set; }

        [Required(ErrorMessage = "Business country is required.")]
        public string businessCountry { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        public string postalCode { get; set; }

        public DateTime BusinessRegDate { get; set; }

        public string ProfileStatus { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        public bool isEmailVerified { get; set; }
    }
}
