namespace mytown.Models.DTO_s
{
   
        public class HubMonthlyCountsDto
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public int Total { get; set; }
            public int Pending { get; set; }
            public int ReachedHub { get; set; }
            public int HandedOver { get; set; }
        }
    
}
