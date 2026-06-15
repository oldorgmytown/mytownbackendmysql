// ===== TravelPlanDto.cs =====
namespace mytown.Models.DTO_s
{
    public class TravelPlanDto
    {
        public int PlanId { get; set; }
        public int TransporterRegId { get; set; }

        public bool IsActive { get; set; }
        public string PlanStatus { get; set; }

        // Route

        // =========================================================
        // START LOCATION
        // =========================================================

        public string StartTown { get; set; }
        public string StartCity { get; set; }
        public string StartState { get; set; }
        public string StartCountry { get; set; }

        // =========================================================
        // DESTINATION LOCATION
        // =========================================================

        public string DestinationTown { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationState { get; set; }
        public string DestinationCountry { get; set; }
        //public string StartLocation { get; set; }
        //public string Destination { get; set; }
        public string PreferredRoute { get; set; }
        public decimal? DistanceKm { get; set; }

        // Schedule
        public DateTime StartDate { get; set; }
        public DateTime ArrivalDate { get; set; }

        // Vehicle
        public string VehicleType { get; set; }
        public string VehicleRegistration { get; set; }
        public string VehicleName { get; set; }

        // Capacity
        public decimal MaxWeightKg { get; set; }
        public decimal? PackageSizeL { get; set; }
        public decimal? PackageSizeW { get; set; }
        public decimal? PackageSizeH { get; set; }
        public int NumberOfPackages { get; set; }
        public bool AcceptsFragile { get; set; }
        public bool AcceptsPerishable { get; set; }

        // Communication
        public string PreferredContact { get; set; }
        public string LanguagePreference { get; set; }
        public bool NotifyNewOrders { get; set; }
        public bool NotifyPayments { get; set; }
    }
}

// ===== TransporterDashboardDto.cs =====
namespace mytown.Models.DTO_s
{
    public class TransporterDashboardDto
    {
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }
        public int TotalDeliveries { get; set; }
        public int ActiveDeliveries { get; set; }
        public decimal TotalEarned { get; set; }
        public string KycStatus { get; set; }
        public bool BankVerified { get; set; }
        public bool HasActivePlan { get; set; }
    }
}

// ===== DeliveryRequestDto.cs =====
namespace mytown.Models.DTO_s
{
    public class DeliveryRequestDto
    {
        public int DeliveryReqId { get; set; }
        public int PlanId { get; set; }
        public int ShopperRegId { get; set; }
        public string ShopperName { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal PackageWeightKg { get; set; }
        public int NumberOfPackages { get; set; }
        public decimal DeliveryFee { get; set; }
        public string PackageTags { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

// ===== ActiveDeliveryDto.cs =====
namespace mytown.Models.DTO_s
{
    public class ActiveDeliveryDto
    {
        public int DeliveryReqId { get; set; }
        public int PlanId { get; set; }
        public int? StoreOrderId { get; set; }   // ← ADD THIS
        public int? OrderId { get; set; }
        public string DeliveryCode { get; set; } // DEL-XXXX
        public string CustomerName { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public int NumberOfPackages { get; set; }
        public decimal PackageWeightKg { get; set; }
        public decimal? PackageLengthCm { get; set; }
        public decimal? PackageWidthCm { get; set; }
        public decimal? PackageHeightCm { get; set; }

        public decimal DeliveryFee { get; set; }
        public string PackageTags { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public string EtaInfo { get; set; }
    }
}

// ===== UpdateDeliveryStatusDto.cs =====
namespace mytown.Models.DTO_s
{
    public class UpdateDeliveryStatusDto
    {
        public int DeliveryReqId { get; set; }
        public int TransporterRegId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}

// ===== ExceptionReportDto.cs =====
namespace mytown.Models.DTO_s
{
    public class ExceptionReportDto
    {
        public int DeliveryReqId { get; set; }
        public int TransporterRegId { get; set; }
        public string ExceptionType { get; set; } // ReportDelay / PackageIssue / CustomerUnreachable / RouteDeviation
        public string Description { get; set; }
    }
}

// ===== TransporterKycDto.cs =====
namespace mytown.Models.DTO_s
{
    public class TransporterKycDto
    {
        public int TransporterRegId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public IFormFile DocumentFile { get; set; }
    }
}

// ===== TransporterBankDto.cs =====
namespace mytown.Models.DTO_s
{
    public class TransporterBankDto
    {
        public int TransporterRegId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string IfscCode { get; set; }
    }
}

// ===== TransporterProfileDto.cs =====
namespace mytown.Models.DTO_s
{
    public class TransporterProfileDto
    {
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Status { get; set; }
        public bool IsEmailVerified { get; set; }
        public string KycStatus { get; set; }
        public bool BankVerified { get; set; }
        public DateTime TransporterRegDate { get; set; }
    }
}

// ===== UpdateTransporterProfileDto.cs =====
namespace mytown.Models.DTO_s
{
    public class UpdateTransporterProfileDto
    {
        public int TransporterRegId { get; set; }
        public string? TransporterName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Town { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}

// ===== AvailableTransporterDto.cs  (for shopper to search) =====
namespace mytown.Models.DTO_s
{
    public class AvailableTransporterDto
    {
        public int PlanId { get; set; }
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }
        public string VehicleType { get; set; }
        public string VehicleName { get; set; }
        // =========================================================
        // START LOCATION
        // =========================================================

        public string StartTown { get; set; }
        public string StartCity { get; set; }
        public string StartState { get; set; }
        public string StartCountry { get; set; }

        // =========================================================
        // DESTINATION LOCATION
        // =========================================================

        public string DestinationTown { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationState { get; set; }
        public string DestinationCountry { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public decimal MaxWeightKg { get; set; }
        public int NumberOfPackages { get; set; }
        public bool AcceptsFragile { get; set; }
        public bool AcceptsPerishable { get; set; }
        public string PreferredContact { get; set; }
        public string PreferredRoute { get; set; }
    }
}

// ===== ShopperDeliveryRequestDto.cs  (shopper sends request to transporter) =====
namespace mytown.Models.DTO_s
{
    public class ShopperDeliveryRequestDto
    {
        public int PlanId { get; set; }
        public int? ShopperRegId { get; set; }
        public int? GuestRegId { get; set; }
        public bool IsGuestOrder { get; set; }
        public int? OrderId { get; set; }
        public int StoreOrderId { get; set; } // link to store order for easier tracking
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal PackageWeightKg { get; set; }
        public int NumberOfPackages { get; set; }
        public string PackageTags { get; set; }
    }
}