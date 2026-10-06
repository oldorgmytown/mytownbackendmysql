using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models.DTO_s
{
    [Table("hub_store_order_verification")]
    public class HubStoreOrderVerification
    {
        [Key][Column("verification_id")] public int VerificationId { get; set; }
        [Column("store_order_id")] public int StoreOrderId { get; set; }
        [Column("hubid")] public int HubId { get; set; }

        [Column("package_verified")] public bool PackageVerified { get; set; }
        [Column("security_check")] public bool SecurityCheck { get; set; }
        [Column("travel_plan_verified")] public bool TravelPlanVerified { get; set; }
        [Column("transporter_verified")] public bool TransporterVerified { get; set; }
        [Column("package_handed_over")] public bool PackageHandedOver { get; set; }

        [Column("remarks")] public string? Remarks { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}
