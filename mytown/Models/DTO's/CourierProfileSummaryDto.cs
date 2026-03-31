namespace mytown.Models.DTO_s
{
    public class CourierProfileSummaryDto
    {
        public string CourierName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

      //  public List<CourierBranchDto> Branches { get; set; }

        public int TodayDeliveries { get; set; }
        public int TotalDeliveries { get; set; }
        public int PendingTasks { get; set; }
    }

}
