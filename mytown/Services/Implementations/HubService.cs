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

        public Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync(int? month, int? year)
        => _hubRepo.GetTransporterStoreOrdersAsync(month, year);

        public Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync(int? month, int? year)
            => _hubRepo.GetTransporterSenderOrdersAsync(month, year);

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

            var saved = await _hubRepo.SaveVerificationAsync(storeOrderId, dto);
            return (true, null, saved);
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

        public Task<HubMonthlyCountsDto> GetStoreOrderCountsAsync(int month, int year)
    => _hubRepo.GetStoreOrderCountsAsync(month, year);

        public Task<HubMonthlyCountsDto> GetSenderOrderCountsAsync(int month, int year)
            => _hubRepo.GetSenderOrderCountsAsync(month, year);
    }
}
