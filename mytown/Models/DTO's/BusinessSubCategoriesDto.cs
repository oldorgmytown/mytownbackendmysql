namespace mytown.Models.DTOs
{
    public class BusinessSubCategoriesDto
    {
        public int BusRegId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryItemDto> SubCategories { get; set; }
    }

    public class SubCategoryItemDto
    {
        public int SubCatId { get; set; }
        public string SubCatName { get; set; }
        public string? SubCatImage { get; set; }
    }
}