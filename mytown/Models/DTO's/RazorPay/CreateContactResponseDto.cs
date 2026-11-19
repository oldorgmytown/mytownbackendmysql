namespace MyTown.DTOs.Razorpay
{
    public class CreateContactResponseDto
    {
        public string Id { get; set; } = string.Empty;

        public string Entity { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool Active { get; set; }
    }
}