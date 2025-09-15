using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class Design
    {
        [Key]
        public int design_id { get; set; }
        public int prod_subcat_id { get; set; }
        public string design_name { get; set; }
    }
}
