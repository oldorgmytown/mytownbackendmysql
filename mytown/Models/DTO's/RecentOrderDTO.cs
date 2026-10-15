namespace mytown.Models.DTO_s
{
    public class RecentOrderDTO
    {
        public int StoreOrderId { get; set; }      
        public string? ShopperName { get; set; }
        public string? ProductName { get; set; }   // first product in that order
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
    }
}
