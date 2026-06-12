using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IMobileAppService
    {
        Task<List<PopularProductDto>> GetPopularProductsAsync();
        Task<List<PopularStoresDto>> GetPopularStoresAsync();
    }
}
