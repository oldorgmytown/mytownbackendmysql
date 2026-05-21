namespace mytown.DTOs
{
    public class CreateServiceProfileDto
    {
        public int BusRegId { get; set; }

        public int BusServId { get; set; }

        public int? YearsOfExperience { get; set; }

        public string? GovtIdDocument { get; set; }

        public string? ProfessionalLicense { get; set; }

        public string? ServiceAvailableLocations { get; set; }

        public string? WorkingDays { get; set; }

        public TimeSpan? WorkingStartTime { get; set; }

        public TimeSpan? WorkingEndTime { get; set; }

        public string? ServiceLogo { get; set; }

        public string? ServiceBanner { get; set; }

        public List<CreateServiceDto> Services { get; set; }
    }

    public class CreateServiceDto
    {
        public int ServSubcatId { get; set; }

        public string ServiceName { get; set; }

        public string? ServiceDescription { get; set; }

        public decimal? InspectionFee { get; set; }

        public decimal? StartingPrice { get; set; }

        public string? EstimatedDuration { get; set; }
    }
}