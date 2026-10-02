namespace mytown.DTOs
{
    public class StorePayoutDetailsDto
    {
        public int StoreOrderId { get; set; }

        public int StoreId { get; set; }

        public decimal Amount { get; set; }

        public string? BeneficiaryId { get; set; }

        public string? BeneficiaryStatus { get; set; }
    }
}