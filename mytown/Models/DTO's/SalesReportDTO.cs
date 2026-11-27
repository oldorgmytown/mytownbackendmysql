namespace mytown.Models.DTO_s
{
    public class SalesReportDTO
    {
        public decimal TotalSales { get; set; }
        public int TotalProductsSold { get; set; }
        public int UniqueOrdersCount { get; set; }
        public int UniqueShoppersCount { get; set; }
        // Growth vs previous month (null = no month/year given, or previous month had 0 -> show "New")
        public decimal? OrdersGrowthPercent { get; set; }
        public decimal? SalesGrowthPercent { get; set; }
        public decimal? CustomersGrowthPercent { get; set; }
        public decimal ProductsChange { get; set; }          // plain number, e.g. -1

        // Extra card info
        public int PendingOrders { get; set; }
        public string? TopSellingItemName { get; set; }
        public long? TopSellingSkuId { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal RetentionPercent { get; set; }
    }
}
