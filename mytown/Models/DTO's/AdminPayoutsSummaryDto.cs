namespace mytown.Models.DTO_s
{
    public class AdminPayoutsSummaryDto
    {

            public int TotalOrders { get; set; }
            public decimal? OrdersGrowthPercent { get; set; }   // null = no orders last month ("New")
            public int DeliveredOrders { get; set; }

            public PayoutCardDto StorePayouts { get; set; }
            public PayoutCardDto P2PLogistics { get; set; }
            public PayoutCardDto CourierLogistics { get; set; }
        
    }


    public class PayoutCardDto
    {
        public decimal TotalAmount { get; set; }      // sum of payout amounts for the month
        public int PayoutCount { get; set; }          // "Total 15 Payouts This month"
        public int SettledCount { get; set; }         // "14 of 15 settled"
        public decimal SettledPercent { get; set; }   // 0-100
    }
}
