namespace mytown.Models.DTO_s
{
   
        public class SalesTrendDto
        {
            public DateTime Date { get; set; }
            public decimal Revenue { get; set; }
            public int TotalOrders { get; set; }      // new
            public int TotalCustomers { get; set; }   // new
        }
    
}
