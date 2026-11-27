namespace mytown.Models.DTO_s
{
    public class TransportHopDto
    {
        public int LegNumber { get; set; }
        public bool IsFinalLeg { get; set; }
        public int TransporterRegId { get; set; }
        public int TransporterPlanId { get; set; }
        public string TransporterName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string FromPin { get; set; } = string.Empty;
        public string ToPin { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }

    public class PlanLeg
    {
        public int PlanId { get; set; }
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string StartLocationPin { get; set; } = string.Empty;
        public string DestinationPin { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
