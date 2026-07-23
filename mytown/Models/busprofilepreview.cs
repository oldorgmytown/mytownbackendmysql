using Stripe;

namespace mytown.Models
{
    public class busprofilepreview
    {
        public int businessprofile_id { get; set; }
        public int BusRegId { get; set; }
        public string Businessname { get; set; }
        public string Businessusername { get; set; }
        public string business_location { get; set; }
         public string business_tagline { get; set; }
        public string business_about { get; set; }
        public string banner_path { get; set; }
        public string logo_path { get; set; }
        public string profile_status { get; set; }
        public string bus_time { get; set; }
        public int BusCatId { get; set; }
        public int BusServId { get; set; }
        public string Businessservice_name { get; set; }
        public string Businesscategory_name { get; set; }

        public string Currency { get; set; }  

        public string BusEmail { get; set; } = string.Empty;
         public string BusPhone { get; set; } = string.Empty;

        //// Pan object
        //public PanData Pan { get; set; }
        //public class PanData
        //{
        //    public int X { get; set; }
        //    public int Y { get; set; }
        //    public float Zoom { get; set; } = 1; // Default zoom value
        //}
    }

}


