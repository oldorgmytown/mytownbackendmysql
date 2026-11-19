using System.ComponentModel.DataAnnotations;

namespace mytown.Models.DTO_s
{

    public class CourierServiceDto
    {
       public int CourierId { get; set; }
        public string CourierServiceName { get; set; }
        public string CourierWebsiteName { get; set; }
        public string CourierEmail { get; set; }
        public string CourierPhone { get; set; }

        public string Address { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public bool IsCity { get; set; }
        public bool IsState { get; set; }

        public string Password { get; set; }
       
        public string AccountHolderName { get; set; }

    
        public string BankName { get; set; }

     
        public string AccountNumber { get; set; }

        [Compare("AccountNumber")]
        public string ConfirmAccountNumber { get; set; }

       
        public string IFSCCode { get; set; }

       
    }



}
