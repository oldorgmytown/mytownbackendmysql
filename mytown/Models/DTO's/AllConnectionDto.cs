namespace mytown.Models.DTO_s
{
    public class AllConnectionDto
    {
        public string Type { get; set; } = "";          // "Shopper" or "Business"
        public int ConnectionId { get; set; }           // id from the matching connections table
        public int Id { get; set; }                     // shopper_reg_id or bus_reg_id
        public string Name { get; set; } = "";          // shopper username or business name
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public int MutualCount { get; set; }            // shoppers only, 0 for businesses
        public DateTime ConnectedOn { get; set; }
    }
}
