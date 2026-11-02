namespace mytown.Models.DTO_s
{
    public class NotConnectedShopperDto
    {
        public int ShopperId { get; set; }
        public string Username { get; set; } = "";
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public int MutualCount { get; set; }
    }
}
