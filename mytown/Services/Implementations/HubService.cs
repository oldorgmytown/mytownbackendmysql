using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.Services.Implementations
{
    public class HubService :IHubService
    {


        private readonly IHubRepository _hubRepo;
        private readonly IEmailService _emailService;

        public HubService(IHubRepository hubRepo, IEmailService emailService)
        {
            _hubRepo = hubRepo;
            _emailService = emailService;
        }

        public Task<List<HubAddressDto>> GetAllHubLocationsAsync()
       => _hubRepo.GetAllHubLocationsAsync();
        public Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync(int? hubId, int? month, int? year)
    => _hubRepo.GetTransporterStoreOrdersAsync(hubId, month, year);

        public Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync(int? hubId, int? month, int? year)
            => _hubRepo.GetTransporterSenderOrdersAsync(hubId, month, year);
        public Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId)
    => _hubRepo.GetVerificationAsync(storeOrderId);

        public async Task<(bool Success, string? Error, HubStoreVerificationDto? Data)> SaveVerificationAsync(
     int storeOrderId, SaveHubVerificationDto dto)
        {
            if (!await _hubRepo.StoreOrderExistsAsync(storeOrderId))
                return (false, "Store order not found.", null);

            // Handover is allowed only after the first four checks
            if (dto.PackageHandedOver &&
                !(dto.PackageVerified && dto.SecurityCheck))
                return (false, "Complete all verification checks before handing over the package.", null);

            try
            {
                var saved = await _hubRepo.SaveVerificationAsync(storeOrderId, dto);
                return (true, null, saved);
            }
            catch (InvalidOperationException ex)          // ✅ NEW: e.g. "This package is not at your hub yet."
            {
                return (false, ex.Message, null);
            }
        }
        public Task<SenderVerificationDto?> GetSenderVerificationAsync(int senderOrderId)
    => _hubRepo.GetSenderVerificationAsync(senderOrderId);

        public async Task<(bool Success, string? Error, SenderVerificationDto? Data)> SaveSenderVerificationAsync(
            int senderOrderId, SaveHubVerificationDto dto)
        {
            if (!await _hubRepo.SenderOrderExistsAsync(senderOrderId))
                return (false, "Sender order not found.", null);

            // Handover is allowed only after the first 2 checks
            if (dto.PackageHandedOver &&
                !(dto.PackageVerified && dto.SecurityCheck ))
                return (false, "Complete all verification checks before handing over the package.", null);

            var saved = await _hubRepo.SaveSenderVerificationAsync(senderOrderId, dto);
            return (true, null, saved);
        }

        public Task<HubStoreOrderDetailsDto?> GetStoreOrderDetailsAsync(int storeOrderId)
    => _hubRepo.GetStoreOrderDetailsAsync(storeOrderId);

        public Task<HubSenderOrderDetailsDto?> GetSenderOrderDetailsAsync(int senderOrderId)
    => _hubRepo.GetSenderOrderDetailsAsync(senderOrderId);

        public Task<HubMonthlyCountsDto> GetStoreOrderCountsAsync(int month, int year, int? hubId)
     => _hubRepo.GetStoreOrderCountsAsync(month, year, hubId);

        public Task<HubMonthlyCountsDto> GetSenderOrderCountsAsync(int month, int year, int? hubId)
            => _hubRepo.GetSenderOrderCountsAsync(month, year, hubId);

        public async Task<TransporterContactDto?> GetPickupTransporterAsync(int storeOrderId, string? hubPin)
        {
            var legs = await _hubRepo.GetLegsByStoreOrderIdAsync(storeOrderId);

            if (legs.Count == 0)
                return null;                       // old order: use the values from the request

            // same stage rule as the hub verification
            var lastDeliveredMiddle = legs.LastOrDefault(l => !l.IsFinalLeg && l.LegStatus == "Delivered");
            int nextLegNumber = (lastDeliveredMiddle?.LegNumber ?? 0) + 1;

            var nextLeg = legs.FirstOrDefault(l => l.LegNumber == nextLegNumber);
            if (nextLeg == null)
                throw new InvalidOperationException("No transporter is waiting to pick up this package.");

            // the package must be at THIS hub: the next leg starts at this hub's pincode
            if (legs.Count > 1 &&
                (string.IsNullOrWhiteSpace(hubPin) || nextLeg.FromPin.Trim() != hubPin.Trim()))
                throw new InvalidOperationException("This package is not at your hub yet.");

            var contact = await _hubRepo.GetTransporterContactAsync(nextLeg.TransporterRegId);
            if (contact == null)
                throw new InvalidOperationException("Transporter not found.");

            return contact;
        }
    }
}
