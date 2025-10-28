using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models.DTO_s
{
    [Table("shopper_alternate_address")]
    public class ShopperAlternateAddress
    {
        [Key]
        [Column("alt_address_id")]
        public int AltAddressId { get; set; }

        [ForeignKey("ShopperRegister")]
        [Column("shopper_reg_id")]
        public int ShopperRegId { get; set; }

        [Required, StringLength(100)]
        [Column("alt_name")]
        public string AltName { get; set; } = string.Empty;

        [Required, Phone, StringLength(15)]
        [Column("alt_phone_number")]
        public string AltPhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(300)]
        [Column("alt_address")]
        public string AltAddress { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("alt_town")]
        public string AltTown { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("alt_city")]
        public string AltCity { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("alt_state")]
        public string AltState { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("alt_country")]
        public string AltCountry { get; set; } = string.Empty;

        [StringLength(10)]
        [Column("alt_postal_code")]
        public string? AltPostalCode { get; set; }

        [StringLength(500)]
        [Column("delivery_notes")]
        public string? DeliveryNotes { get; set; }

        public ShopperRegister? ShopperRegister { get; set; }
    }
}
