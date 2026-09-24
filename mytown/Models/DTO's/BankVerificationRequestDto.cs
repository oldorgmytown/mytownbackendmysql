namespace mytown.Models.DTO_s
{
    public class BankVerificationRequestDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string Ifsc { get; set; } = string.Empty;
    }
}
