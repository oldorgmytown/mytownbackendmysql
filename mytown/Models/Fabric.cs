using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class Fabric
    {
        [Key]
        public int fabric_id { get; set; }
        public int prod_subcat_id { get; set; }
        public string fabric_name { get; set; }
    }

    

}
