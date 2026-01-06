using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class BusinessDashboardService : IBusinessDashboardService
    {
        private readonly IBusinessDashboardRepository _repository;

        public BusinessDashboardService(IBusinessDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId)
            => _repository.GetNewOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId)
            => _repository.GetPendingOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId)
            => _repository.GetInProgressOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId)
            => _repository.GetCompletedOrdersAsync(storeId);

        public async Task<List<BusinessProductDashboardDto>> GetProductsAsync(int storeId)
        {
            return await _repository.GetProductsForDashboardAsync(storeId);
        }
    }


}




