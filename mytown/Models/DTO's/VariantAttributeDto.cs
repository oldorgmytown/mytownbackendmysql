namespace mytown.Models.DTO_s
{
    public class VariantAttributeDto
    {
        public long AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public long? AttributeValueId { get; set; }
        public string? AttributeValue { get; set; }
    }
}