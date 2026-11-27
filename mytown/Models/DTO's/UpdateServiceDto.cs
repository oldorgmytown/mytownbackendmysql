public class UpdateServiceDto
{
    public int ServiceId { get; set; }
    public string? ServiceTypeDescription { get; set; }

    public decimal? InspectionFee { get; set; }

    public decimal? StartingPrice { get; set; }

    public string? EstimatedDuration { get; set; }

    public string? ServiceTypeImage { get; set; }
}