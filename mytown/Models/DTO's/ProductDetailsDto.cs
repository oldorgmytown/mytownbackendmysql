namespace mytown.Models.DTO_s
{
    public class ProductDetailsDto
    {
        public int ProdSubcatId { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IEnumerable<Fabric> Fabrics { get; set; }
        public IEnumerable<Design> Designs { get; set; }
    }

}
