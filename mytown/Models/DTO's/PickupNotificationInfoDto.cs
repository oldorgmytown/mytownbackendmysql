namespace mytown.Models.DTO_s
{
    public class PickupNotificationInfo
    {
        public int OrderId { get; set; }
        public int StoreOrderId { get; set; }
        public int? ShopperRegId { get; set; }   // null for guest orders
        public string? Email { get; set; }       // shopper or guest (used later for email)
        public string? CustomerName { get; set; }
        public string StoreName { get; set; } = "";
        public string StoreAddress { get; set; } = "";
        public string? StorePhone { get; set; }
    }
}
