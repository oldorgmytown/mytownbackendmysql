namespace mytown.Models.DTO_s
{
    public class CourierPayoutDashboardDto
    {
        public int StoreOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string? TransferUtr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}