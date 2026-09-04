namespace mytown.Models.DTO_s
{
    public class ProductSearchRequestDto
    {
        public int BusRegId { get; set; }

        public string? Search { get; set; }

        public int? ProdSubcatId { get; set; }

        public int? ProductGroupId { get; set; }

        public int? ProdTypeId { get; set; }

        public List<long> AttributeValueIds { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}