namespace mytown.Models.DTO_s
{
    public class SearchResultDto
    {
        public List<BusinessProfile> Stores { get; set; } = new();
        public List<ProdcVariantforShopperDto> Products { get; set; } = new();
        public List<string> Colors { get; set; } = new List<string>();

        public int StoreCount { get; set; }
        public int ProductCount { get; set; }
    }
}
