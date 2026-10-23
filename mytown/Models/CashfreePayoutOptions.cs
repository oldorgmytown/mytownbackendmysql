namespace MyTown.Models
{
    //totprod
    public class CashfreePayoutOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = string.Empty;
    }
}