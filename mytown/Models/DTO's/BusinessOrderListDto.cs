namespace mytown.Models.DTO_s
{
    public class BusinessOrderListDto
    {
        public int StoreOrderId { get; set; }

        public int OrderId { get; set; } 
        public string Status { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
