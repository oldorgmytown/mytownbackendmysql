namespace mytown.Models.DTO_s
{
    public class ProductMasterNamesDto
    {
        public List<ProductCategoryDto> ProductCategories { get; set; } = new();
        public List<ProductGroupDto> ProductGroups { get; set; } = new();
        public List<ProductTypeDto> ProductTypes { get; set; } = new();
    }

    public class ProductCategoryDto
    {
        public long BusCatId { get; set; }
        public string BusCatName { get; set; } = string.Empty;
    }

    public class ProductGroupDto
    {
        public long ProductGroupId { get; set; }
        public string ProductGroupName { get; set; } = string.Empty;
    }

    public class ProductTypeDto
    {
        public long ProdTypeId { get; set; }
        public string ProductTypeName { get; set; } = string.Empty;
    }
}