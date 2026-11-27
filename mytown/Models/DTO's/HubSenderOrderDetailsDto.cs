namespace mytown.Models.DTO_s
{
    public class HubSenderOrderDetailsDto
    {
        public int SenderOrderId { get; set; }
        public string OrderStatus { get; set; }
        public string HubStatus { get; set; }

        public PackageDetailsDto? Package { get; set; }
        public SenderDetailsDto Sender { get; set; }
        public TransporterDetailsDto Transporter { get; set; }
        public TravelDetailsDto? Travel { get; set; }
        public ChecklistDto Checklist { get; set; }
    }

    public class SenderDetailsDto
    {
        public int SenderRegId { get; set; }
        public string SenderName { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
