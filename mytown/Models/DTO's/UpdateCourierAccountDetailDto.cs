namespace mytown.Models.DTO_s
{
    public class UpdateCourierAccountDetailDto
    {
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public bool IsTermsAccepted { get; set; }
    }
}
