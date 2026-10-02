namespace mytown.Models.DTO_s
{
    public class TransporterRegisterDto
    {
        public int TransporterId { get; set; }
        public string TransporterName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string? Status { get; set; }
         public bool IsEmailVerified { get; set; }
         public DateTime TransporterRegDate { get; set; }

        // Bank Account Details
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public bool IsTermsAccepted { get; set; }
    }
}
