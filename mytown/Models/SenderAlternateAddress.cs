using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("sender_alternate_address")]
    public class SenderAlternateAddress
    {
        [Key]
        [Column("alt_address_id")]
        public int AltAddressId { get; set; }

        [Column("sender_reg_id")]
        public int SenderRegId { get; set; }

        [Required]
        [Column("alt_name")]
        public string AltName { get; set; }

        [Required]
        [Column("alt_phone_number")]
        public string AltPhoneNumber { get; set; }

        [Required]
        [Column("alt_address")]
        public string AltAddress { get; set; }

        [Required]
        [Column("alt_town")]
        public string AltTown { get; set; }

        [Required]
        [Column("alt_city")]
        public string AltCity { get; set; }

        [Required]
        [Column("alt_state")]
        public string AltState { get; set; }

        [Required]
        [Column("alt_country")]
        public string AltCountry { get; set; }

        [Column("alt_postal_code")]
        public string AltPostalCode { get; set; }

        [Column("delivery_notes")]
        public string DeliveryNotes { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}