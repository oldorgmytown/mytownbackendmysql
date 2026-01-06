using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IBusinessDashboardService
    {
        Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId);

        Task<List<BusinessProductDashboardDto>> GetProductsAsync(int storeId);
    }
}
