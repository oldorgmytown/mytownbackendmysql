namespace mytown.Models.DTO_s
{
    //movetoprod
    public class BankVerificationResponseDto
    {
        public bool Success { get; set; }

        public string? AccountHolderName { get; set; }

        public string? BankName { get; set; }

        public string? Message { get; set; }
    }
}
