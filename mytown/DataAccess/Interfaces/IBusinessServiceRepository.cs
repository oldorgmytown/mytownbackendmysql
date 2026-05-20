namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessServiceRepository
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
    }
}