namespace mytown.Models.DTO_s
{
    public class BusinessSalesSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Currency { get; set; }

        public List<SalesTrendDto> RevenueTrend { get; set; }
    }
}
