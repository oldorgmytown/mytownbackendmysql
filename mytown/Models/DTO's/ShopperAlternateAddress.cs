using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models.DTO_s
{
    public class ShopperAlternateAddress
    {
        [Key]
        public int AltAddressId { get; set; }

        [ForeignKey("ShopperRegister")]
        public int ShopperRegId { get; set; }

        [Required, StringLength(100)]
        public string AltName { get; set; } = string.Empty;

        [Required, Phone, StringLength(15)]
        public string AltPhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string AltAddress { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string AltTown { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string AltCity { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string AltState { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string AltCountry { get; set; } = string.Empty;

        [StringLength(10)]
        public string? AltPostalCode { get; set; }

        [StringLength(500)]
        public string? DeliveryNotes { get; set; }

        public ShopperRegister? ShopperRegister { get; set; }
    }
}
