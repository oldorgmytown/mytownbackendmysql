using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{

    public class ProductType
    {
        [Key]
        public int prod_type_id { get; set; }

        // Keep this column name the same as in `products`
        public int prod_subcat_id { get; set; }

        public string prod_type_name { get; set; }
    }
}
