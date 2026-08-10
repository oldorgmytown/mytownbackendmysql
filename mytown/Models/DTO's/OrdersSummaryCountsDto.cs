namespace mytown.Models.DTO_s
{
    public class OrdersSummaryCountsDto
    {
        public int TotalOrders { get; set; }
        public int Pending { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }
}