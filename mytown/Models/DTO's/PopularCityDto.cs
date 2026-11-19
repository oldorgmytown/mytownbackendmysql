namespace mytown.Models.DTO_s
{
    public class PopularCityDto
    {
        public string City { get; set; }
        public string Country { get; set; }
        public int StoreCount { get; set; }
        public int TownCount { get; set; }
        public string? ImageFileName { get; set; }
    }

    public class CountryDto
    {
        public string Country { get; set; }
        public int StoreCount { get; set; }
        public string? ImageUrl { get; set; }
      //  public int StoreCount { get; set; }

    }
}