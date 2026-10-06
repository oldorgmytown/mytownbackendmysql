using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("hub_sender_order_verification")]
    public class HubSenderOrderVerification
    {
        [Key][Column("verification_id")] public int VerificationId { get; set; }
        [Column("sender_order_id")] public int SenderOrderId { get; set; }
        [Column("hubid")] public int HubId { get; set; }
        [Column("hub_status")] public string HubStatus { get; set; } = "New";

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
