using System.ComponentModel.DataAnnotations;

namespace mytown.Models.DTO_s
{
    public class ResendemailVerificationDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
