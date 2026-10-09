namespace mytown.Models.DTO_s
{
    public class TownListDto
    {
        public string TownName { get; set; }

        public int ActiveStoreCount { get; set; }

        public List<string> PopularStores { get; set; }
    }
}