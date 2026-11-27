namespace mytown.Models.DTO_s
{
    public class PopularProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int BusRegId { get; set; }
        public string StoreName { get; set; }
        public string StoreCity { get; set; }
        public string StoreCountry { get; set; }

        public int SkuId { get; set; }

        public decimal Cost { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? DiscountPercent { get; set; }

        public string ImageName { get; set; }

        public int TotalOrders { get; set; }
    }
}
