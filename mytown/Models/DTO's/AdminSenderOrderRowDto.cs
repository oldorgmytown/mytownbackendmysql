namespace mytown.Models.DTO_s
{
    public class AdminSenderOrderRowDto
    {
        public int SenderOrderId { get; set; }          // shows as "#490"
        public DateTime PickupDate { get; set; }

        public string? SenderName { get; set; }
        public string? SenderLocation { get; set; }     // pickup town, city

        public string? TransporterName { get; set; }    // null = not assigned yet
        public string? TransporterLocation { get; set; }

        public string? ProductName { get; set; }

        public decimal? PayoutAmount { get; set; }      // TransporterCharges, null if no transporter
        public string? PayoutStatus { get; set; }       // "Paid" / "Pending", null if no transporter

        public string? DeliveryStatus { get; set; }
    }
}