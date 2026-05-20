using System;
using mytown.Models;
namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessServiceRepository
    {
        Task<List<BusinessService>> GetAllServicesAsync();
    }
}