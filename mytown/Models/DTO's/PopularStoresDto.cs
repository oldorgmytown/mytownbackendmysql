namespace mytown.Models.DTO_s
{
    public class PopularStoresDto
    {
        public int BusRegId { get; set; }
        public string StoreName { get; set; }

        public int BuscatId { get; set; }
        public string CategoryName { get; set; }

        public string StoreLogo { get; set; }
        public string StoreBanner { get; set; }

        public string Location { get; set; }

        public int TotalOrders { get; set; }
    }
}
