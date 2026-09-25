using System.Text.Json.Serialization;

namespace mytown.Models.DTO_s
{
    //toprod
    public class CashfreeBeneficiaryResponse
    {
        [JsonPropertyName("beneficiary_id")]
        public string BeneficiaryId { get; set; }

        [JsonPropertyName("beneficiary_name")]
        public string BeneficiaryName { get; set; }

        [JsonPropertyName("beneficiary_status")]
        public string BeneficiaryStatus { get; set; }
    }
}