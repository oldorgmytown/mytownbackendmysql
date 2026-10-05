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

        public Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync()
        => _hubRepo.GetTransporterStoreOrdersAsync();
    }
}
