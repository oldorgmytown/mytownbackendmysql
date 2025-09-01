using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(500)]
        public string FileName { get; set; }   // store blobfilename

        public int SortOrder { get; set; }     //  maintain order of images

        public virtual products Product { get; set; }
    }
}
