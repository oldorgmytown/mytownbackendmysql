using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace mytown.Models
{
 
    [Table("hubdetails")]
    public class HubDetail
    {
        [Key][Column("hubid")] public int HubId { get; set; }
        [Column("hubaddressid")] public int HubAddressId { get; set; }
        [Column("hubname")] public string HubName { get; set; }
        [Column("hubemail")] public string HubEmail { get; set; }
        [Column("hubpassword")] public string HubPassword { get; set; }
        [Column("addressline")] public string? AddressLine { get; set; }
        [Column("town")] public string? Town { get; set; }
        [Column("city")] public string? City { get; set; }
        [Column("state")] public string? State { get; set; }
        [Column("country")] public string? Country { get; set; }
        [Column("pin")] public string? Pin { get; set; }
        [Column("phone")] public string? Phone { get; set; }
    }
}
