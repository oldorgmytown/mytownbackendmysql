namespace mytown.Models.DTO_s
{
    public class StoreSalesHistoryDto
    {

        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AvgOrderValue { get; set; }
        public int TotalActiveCustomers { get; set; }
    }
}
