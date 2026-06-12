using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IMobileAppRepository
    {
        Task<List<PopularProductDto>> GetPopularProductsAsync();
        Task<List<PopularStoresDto>> GetPopularStoresAsync();
        Task<List<TownStoreCountDto>> GetExploreTownsAsync();


    }
}
