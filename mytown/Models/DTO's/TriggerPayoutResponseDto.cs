namespace mytown.DTOs
{
    public class TriggerPayoutResponseDto
    {
        public bool Success { get; set; }

        public string? TransferId { get; set; }

        public string? CfTransferId { get; set; }

        public string? Status { get; set; }

        public string? StatusCode { get; set; }

        public string? StatusDescription { get; set; }

        public decimal? TransferAmount { get; set; }

        public string? TransferMode { get; set; }

        public string? TransferUtr { get; set; }

        public string? Message { get; set; }
    }
}