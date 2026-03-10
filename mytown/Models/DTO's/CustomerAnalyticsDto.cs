namespace mytown.Models.DTO_s
{
    public class CustomerAnalyticsDto
    {
       
            public int CustomersVisitedAndPurchased { get; set; }
            public int CustomersVisitedButNotPurchased { get; set; }

            public int TotalCustomers { get; set; }
            public int TotalVisits { get; set; }
            public int RepeatingCustomers { get; set; }
            public decimal ConversionRate { get; set; }

            public List<FrequentCustomerDto> FrequentCustomers { get; set; }
            public List<CustomerDto> CustomersWhoPurchased { get; set; }
        
    }
}

