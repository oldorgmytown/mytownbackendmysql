namespace mytown.Models.DTO_s
{
    public class MobileSignupDto
    {
        public string Role { get; set; }
        public string BusinessName { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Town { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? BusinessType { get; set; }
    }

    public class MobileSendOtpDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class MobileVerifyOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string Role { get; set; }
    }
}