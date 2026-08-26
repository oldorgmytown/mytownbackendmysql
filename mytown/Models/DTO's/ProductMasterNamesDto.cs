namespace mytown.Models.DTO_s
{
    public class ProductMasterNamesDto
    {
        public List<ProductSubCategoryDto> ProductSubCategories { get; set; } = new();
        public List<ProductGroupDto> ProductGroups { get; set; } = new();
        public List<ProductTypeDto> ProductTypes { get; set; } = new();
    }

    public class ProductSubCategoryDto
    {
        public long ProdSubcatId { get; set; }
        public string ProdSubCatName { get; set; } = string.Empty;
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