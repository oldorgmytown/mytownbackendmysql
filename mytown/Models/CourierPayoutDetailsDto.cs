namespace mytown.Models
{
    public class CourierPayoutDetailsDto
    {
        public int StoreOrderId { get; set; }
        public int CourierId { get; set; }
        public decimal Amount { get; set; }
        public string? BeneficiaryId { get; set; }
    }
}
