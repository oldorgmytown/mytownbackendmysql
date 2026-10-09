namespace mytown.Models.DTO_s
{
    //push to prod
    public class TransporterPayoutDashboardDto
    {
        public int? StoreOrderId { get; set; }
        public int? SenderOrderId { get; set; }
        public string OrderType { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public string Bankname { get; set; }
        public string AccountNumber { get; set; }
        public string CF_TransferId { get; set; }
        public string? TransferUtr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}