namespace mytown.DTOs
{
    public class ReceivedRequestDto
    {
        public int ConnectionId { get; set; }
        public int ShopperId { get; set; }          // the shopper who sent the request
        public string Username { get; set; } = "";
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City{ get; set; } = "";
        public string State{ get; set; } = "";
        public int MutualCount { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}