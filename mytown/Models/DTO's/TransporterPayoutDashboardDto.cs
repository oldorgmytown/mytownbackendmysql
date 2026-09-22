namespace mytown.Models.DTO_s
{
    public class TransporterPayoutDashboardDto
    {
        public int StoreOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }

        public string Bankname { get; set; }

        public string AccountNumber { get; set; }
        public string CF_TransferId { get; set; }
        public string? TransferUtr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}