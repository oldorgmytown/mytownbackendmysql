using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class CreatePaymentIntentRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        //[Required(ErrorMessage = "CountryName is required.")]
        //[StringLength(100)]
        //public string CountryName { get; set; }

        //[Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        //public long Amount { get; set; } // smallest unit (paise / cents)
    }
}
