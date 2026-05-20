namespace mytown.Services.Interfaces
{
    public interface IBusinessServiceService
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
    }
}
