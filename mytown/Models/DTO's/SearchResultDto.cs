namespace mytown.Models.DTO_s
{
    public class SearchResultDto
    {
        public List<businessprofile> Stores { get; set; } = new();
        public List<ProdcVariantforShopperDto> Products { get; set; } = new();
        public List<string> Colors { get; set; } = new List<string>();

        public int StoreCount { get; set; }
        public int ProductCount { get; set; }
    }
}
