using System.ComponentModel.DataAnnotations;

public class ContactEnquiryDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Enter a valid phone number.")]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? UserRole { get; set; }

    [Required(ErrorMessage = "Message is required.")]
    [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
    public string Message { get; set; } = string.Empty;
}