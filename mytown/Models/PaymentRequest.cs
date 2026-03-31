using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class PaymentRequestDto
    {
        [Required(ErrorMessage = "CountryName is required.")]
        [StringLength(100, ErrorMessage = "CountryName cannot exceed 100 characters.")]
        public string CountryName { get; set; }

        [Required(ErrorMessage = "CurrencySymbol is required.")]
        [StringLength(10, ErrorMessage = "CurrencySymbol cannot exceed 10 characters.")]
        public string CurrencySymbol { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public long Amount { get; set; }
    }
}
