using System.Text.Json.Serialization;

namespace mytown.Models
{
    //toprod
    public class CashfreePayoutApiResponse
    {
        [JsonPropertyName("transfer_id")]
        public string? TransferId { get; set; }

        [JsonPropertyName("cf_transfer_id")]
        public string? CfTransferId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("status_code")]
        public string? StatusCode { get; set; }

        [JsonPropertyName("status_description")]
        public string? StatusDescription { get; set; }

        [JsonPropertyName("transfer_amount")]
        public decimal? TransferAmount { get; set; }

        [JsonPropertyName("transfer_mode")]
        public string? TransferMode { get; set; }

        [JsonPropertyName("transfer_utr")]
        public string? TransferUtr { get; set; }
    }
}