namespace mytown.Models.DTO_s
{
    public class ShopperDBOrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string ShippingStatus { get; set; }
    }
}
