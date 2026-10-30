namespace mytown.Models.DTO_s
{
    public class ShopperBusinessConnectionDto
    {
        public int BusConnectionId { get; set; }
        public int BusRegId { get; set; }
        public string BusinessName { get; set; } = "";
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public DateTime ConnectedOn { get; set; }
    }
}
