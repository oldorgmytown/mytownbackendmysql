namespace mytown.Models.DTO_s
{
    public class BusinessLocationCountsDto
    {
        public int TotalCountries { get; set; }
        public int TotalTowns { get; set; }
        public List<CountryTownCountDto> CountryBreakdown { get; set; } = new();
    }

    public class CountryTownCountDto
    {
        public string Country { get; set; }
        public int TownCount { get; set; }
    }
}