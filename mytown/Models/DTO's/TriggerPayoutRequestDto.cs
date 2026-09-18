namespace mytown.Models.DTO_s
{
    public class TriggerPayoutRequestDto
    {
        public string BeneficiaryId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransferId { get; set; } = string.Empty;
        public string? TransferMode { get; set; }
        public string? Remarks { get; set; }
    }
}
