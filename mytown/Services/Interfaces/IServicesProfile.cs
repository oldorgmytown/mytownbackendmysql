using mytown.Models;

namespace mytown.Services.Interfaces
{
    public interface IServicesProfile
    {
        Task<List<BusinessService>> GetAllServicesAsync();
    }
}