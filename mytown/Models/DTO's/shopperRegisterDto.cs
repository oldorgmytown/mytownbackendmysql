using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class ShopperRegisterDto
    {
        public int ShopperRegId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public string PhotoName { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public DateTime ShopperRegDate { get; set; }
    }
}
