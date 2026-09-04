namespace mytown.Models.DTO_s
{
    public class CreateCashfreeBeneficiaryRequest
    {
        public string BeneficiaryId { get; set; }
        public string BeneficiaryName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIfsc { get; set; }

        public string BeneficiaryEmail { get; set; }
        public string BeneficiaryPhone { get; set; }
        public string BeneficiaryCountryCode { get; set; } = "+91";
    }
}
