using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class CityImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        public string? ImageFileName { get; set; }
    }
}