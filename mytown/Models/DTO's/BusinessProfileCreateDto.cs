using System.ComponentModel.DataAnnotations;

namespace mytown.Models.DTO_s
{
    public class BusinessProfileCreateDto
    {
        [Required(ErrorMessage = "BusRegId is required.")]
        public int BusRegId { get; set; }

        [Required(ErrorMessage = "Business username is required.")]
        [StringLength(100, ErrorMessage = "Business username cannot exceed 100 characters.")]
        public string? BusinessUsername { get; set; }

        [Required(ErrorMessage = "Business location is required.")]
        [StringLength(250, ErrorMessage = "Business location cannot exceed 250 characters.")]
        public string? BusinessLocation { get; set; }

        [Required(ErrorMessage = "Business about/description is required.")]
        [StringLength(500, ErrorMessage = "Business about section cannot exceed 500 characters.")]
        public string? BusinessAbout { get; set; }

        [Required(ErrorMessage = "Business category ID is required.")]
        public int? Buscatid { get; set; }

        [Required(ErrorMessage = "Profile status is required.")]
        public string? ProfileStatus { get; set; }
    }
}
