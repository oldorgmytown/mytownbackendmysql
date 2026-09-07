namespace mytown.DTOs
{
    public class SenderOrderSummaryRequestDto
    {
        public int SenderOrderId { get; set; }

        public int TransporterRegId { get; set; }

        public int TransporterPlanId { get; set; }
    }
}