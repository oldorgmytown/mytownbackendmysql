namespace mytown.Models
{
    public class PendingTransporterVerification
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string JsonPayload { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
