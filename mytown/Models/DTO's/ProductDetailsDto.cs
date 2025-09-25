namespace mytown.Models.DTO_s
{
    public class ProductDetailsDto
    {
        //public List<(int ProdSubcatId, string ProdSubcatName)> ProductSubCategories { get; set; } = new();
        public int ProdSubcatId { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IEnumerable<Fabric> Fabrics { get; set; }
        public IEnumerable<Design> Designs { get; set; }
        public List<Product_Sizes> Sizes { get; set; }
    }

}
