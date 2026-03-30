using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    [Table("transporter_exception_reports")]
    public class TransporterExceptionReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("report_id")]
        public int ReportId { get; set; }

        [Required]
        [Column("delivery_req_id")]
        public int DeliveryReqId { get; set; }

        [Required]
        [Column("transporter_reg_id")]
        public int TransporterRegId { get; set; }

        // ReportDelay / PackageIssue / CustomerUnreachable / RouteDeviation
        [Required]
        [Column("exception_type", TypeName = "varchar(50)")]
        public string ExceptionType { get; set; }

        [Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Column("reported_at")]
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        [Column("is_resolved")]
        public bool IsResolved { get; set; } = false;

        [ForeignKey(nameof(DeliveryReqId))]
        public TransporterDeliveryRequest DeliveryRequest { get; set; }

        [ForeignKey(nameof(TransporterRegId))]
        public TransporterRegister TransporterRegister { get; set; }
    }
}