namespace mytown.Models.DTO_s
{
    public class ProductAttributeDto
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int ProdSubcatId { get; set; }
        public int BusCatId { get; set; }

        public List<ProductAttributeValueDto> Values { get; set; }
            = new List<ProductAttributeValueDto>();
    }

    public class ProductAttributeValueDto
    {
        public int AttributeValueId { get; set; }
        public string AttributeValue { get; set; }
    }
}
