namespace mytown.DTOs
{
    public class MatchingTransporterDto
    {
        public int PlanId { get; set; }

        public int TransporterRegId { get; set; }

        public string TransporterName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string VehicleType { get; set; }

        public string VehicleName { get; set; }

        public decimal MaxWeightKg { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public string PreferredContact { get; set; }
    }
}