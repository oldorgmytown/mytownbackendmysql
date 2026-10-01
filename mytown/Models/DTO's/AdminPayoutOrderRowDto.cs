namespace mytown.Models.DTO_s
{
    public class AdminPayoutOrderRowDto
    {

        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }                 // shows as "#490"
        public DateTime OrderDate { get; set; }

        public string? ShopperName { get; set; }
        public bool IsGuestOrder { get; set; }           // show a "Guest" tag next to the name
        public string? ShopperLocation { get; set; }
        public string? StoreName { get; set; }
        public string? StoreLocation { get; set; }

        public decimal Amount { get; set; }

        // null amount/status = no payout row yet (show "—")
        public decimal? StorePayoutAmount { get; set; }
        public string? StorePayoutStatus { get; set; }          // "Paid" / "Pending"
        public decimal? CourierPayoutAmount { get; set; }
        public string? CourierPayoutStatus { get; set; }
        public decimal? TransporterPayoutAmount { get; set; }
        public string? TransporterPayoutStatus { get; set; }

        public string? ShippingStatus { get; set; }
    }

    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new();
    }
}

