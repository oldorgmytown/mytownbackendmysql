using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    public class businessprofile
    {
        [Key]
        public int businessprofile_id { get; set; }

        [ForeignKey("BusinessRegister")]
        public int BusRegId { get; set; }
     
        public string? BusinessUsername { get; set; }
        public string? business_location { get; set; }
        public string? business_about { get; set; }
        public string? banner_path { get; set; }
        public string? logo_path { get; set; }
        public string? profile_status { get; set; }
       // public string? bus_time { get; set; }
        public int BusCatId { get; set; }
        public int BusServId { get; set; }
        //public string? Businessservice_name { get; set; }
        //public string? Businesscategory_name { get; set; }

        public DateTime? approved_date { get; set; }

        //// Store pan data as separate columns
        //public int image_positionx { get; set; } = 0;
        //public int image_positiony { get; set; } = 0;
        //public float zoom { get; set; } = 1; // Safe default

        [JsonIgnore]
        public virtual BusinessRegister? BusinessRegister { get; set; }
    }

}
