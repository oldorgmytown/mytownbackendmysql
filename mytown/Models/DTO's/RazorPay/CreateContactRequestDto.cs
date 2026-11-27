namespace MyTown.DTOs.Razorpay
{
    public class CreateContactRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public string Type { get; set; } = "vendor";
    }
}