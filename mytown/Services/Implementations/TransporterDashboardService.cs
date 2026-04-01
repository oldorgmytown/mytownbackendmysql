// ===== TransporterDashboardService.cs =====
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Azure.Storage.Blobs;

namespace mytown.Services.Implementations
{
    public class TransporterDashboardService : ITransporterDashboardService
    {
        private readonly ITransporterDashboardRepository _repo;
        private readonly IConfiguration _config;

        public TransporterDashboardService(
            ITransporterDashboardRepository repo,
            IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public Task<TransporterDashboardDto> GetDashboardSummaryAsync(int transporterRegId)
            => _repo.GetDashboardSummaryAsync(transporterRegId);

        public Task<TravelPlanDto?> GetActivePlanAsync(int transporterRegId)
            => _repo.GetActivePlanAsync(transporterRegId);

        public Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto)
            => _repo.SaveTravelPlanAsync(dto);

        public Task<bool> DeactivatePlanAsync(int planId, int transporterRegId)
            => _repo.DeactivatePlanAsync(planId, transporterRegId);

        public Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
            string fromLocation, string toLocation, DateTime travelDate)
            => _repo.SearchAvailableTransportersAsync(fromLocation, toLocation, travelDate);

        public async Task<(bool success, string message, int deliveryReqId)> CreateDeliveryRequestAsync(
            ShopperDeliveryRequestDto dto)
        {
            try
            {
                var result = await _repo.CreateDeliveryRequestAsync(dto);
                return (true, "Delivery request sent to transporter.", result.DeliveryReqId);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }

        public Task<List<DeliveryRequestDto>> GetPendingRequestsAsync(int transporterRegId)
            => _repo.GetPendingRequestsAsync(transporterRegId);

        public Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId)
    => _repo.GetActiveDeliveryAsync(transporterRegId);

        public Task<bool> AcceptDeliveryRequestAsync(int deliveryReqId, int transporterRegId)
            => _repo.AcceptDeliveryRequestAsync(deliveryReqId, transporterRegId);

        public Task<List<TravelPlanDto>> GetAllPlansAsync(int transporterRegId)
            => _repo.GetAllPlansAsync(transporterRegId);

        public Task<bool> UpdateDeliveryStatusAsync(UpdateDeliveryStatusDto dto)
            => _repo.UpdateDeliveryStatusAsync(dto);

        public Task<List<ActiveDeliveryDto>> GetCompletedDeliveriesAsync(int transporterRegId)
            => _repo.GetCompletedDeliveriesAsync(transporterRegId);

        public Task<bool> SubmitExceptionReportAsync(ExceptionReportDto dto)
            => _repo.SubmitExceptionReportAsync(dto);

        public async Task<(bool success, string message)> SubmitKycAsync(TransporterKycDto dto)
        {
            if (dto.DocumentFile == null || dto.DocumentFile.Length == 0)
                return (false, "Document file is required.");

            // Upload to Azure Blob
            string fileName = await UploadToBlobAsync(dto.DocumentFile, "kyc-docs");

            await _repo.SubmitKycAsync(dto.TransporterRegId, dto.DocumentType, dto.DocumentNumber, fileName);
            return (true, "KYC submitted successfully. Pending review.");
        }

        public async Task<(bool success, string message)> SubmitBankDetailsAsync(TransporterBankDto dto)
        {
            await _repo.SubmitBankDetailsAsync(dto);
            return (true, "Bank details submitted successfully.");
        }

        public Task<TransporterProfileDto?> GetProfileAsync(int transporterRegId)
            => _repo.GetProfileAsync(transporterRegId);

        public Task<bool> UpdateProfileAsync(UpdateTransporterProfileDto dto)
            => _repo.UpdateProfileAsync(dto);

        public async Task<bool> UpdatePasswordAsync(int transporterRegId, string currentPassword, string newPassword)
        {
            var profile = await _repo.GetProfileAsync(transporterRegId);
            if (profile == null) return false;

            // Need to verify current password - fetch transporter entity
            // For now, hash new password and update
            string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _repo.UpdatePasswordAsync(transporterRegId, hashed);
        }

        private async Task<string> UploadToBlobAsync(IFormFile file, string folder)
        {
            var connStr = _config["AzureBlobStorage:ConnectionString"];
            var containerName = _config["AzureBlobStorage:ContainerName"];
            var blobClient = new BlobServiceClient(connStr);
            var container = blobClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            string uniqueName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
            var blob = container.GetBlobClient(uniqueName);
            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, overwrite: true);
            return uniqueName;
        }
    }
}