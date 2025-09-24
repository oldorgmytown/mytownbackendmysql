namespace mytown.Models.DTO_s
{
    public class ProductSizeMeasurementDto
    {
       
            public int MeasurementId { get; set; }
            public int SizeId { get; set; }
            public decimal? Length { get; set; }
            public decimal? Height { get; set; }
            public decimal? Width { get; set; }
            public decimal? Weight { get; set; }
            public string? Unit { get; set; }
       

    }
}
