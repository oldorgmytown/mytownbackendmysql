using mytown.DTOs.ProductsNew;

namespace mytown.Services.Interfaces
{
    public interface IProductsNewService
    {
        Task<long> CreateProductAsync(
            CreateProductNewRequest request);
    }
}