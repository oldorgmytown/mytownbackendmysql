namespace mytown.Models.DTO_s
{
    public class TransporterPayoutDetailsDto
    {
        public int StoreOrderId { get; set; }

        public int TransporterRegId { get; set; }

        public decimal Amount { get; set; }

        public string? BeneficiaryId { get; set; }
    }
}