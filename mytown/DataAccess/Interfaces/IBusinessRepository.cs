using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.DataAccess
{
    public interface IBusinessRepository
    {
        Task<bool> IsEmailTaken(string email);
        Task SavePendingVerification(PendingBusinessVerification pending);       
        Task<PendingBusinessVerification> FindPendingVerificationByToken(string token);    
        Task DeletePendingVerification(string token);     
        Task RegisterBusiness(BusinessRegister business);

        //FOR resend email verifcation
        Task<BusinessVerification> FindPendingVerificationByEmail(string email);
        Task RemoveVerification(BusinessVerification verification);
        Task<BusinessRegister> GetBusinessByIdAsync(int busRegId);
        Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories();
        Task<ActionResult<IEnumerable<businessservices>>> GetBusinessServices();
        Task<IEnumerable<product_sub_categories>> BusinessSubCategoriesforStores(int buscatId);
        Task AddOrUpdateSubcategoryImageAsync(subcategoryimages_busregid subcategoryImage);
        Task<List<subcategoryimages_busregid>> GetSubcategoryImagesByBusRegIdAsync(int busRegId);
        Task<products> CreateProductAsync(products product);
        Task<products> GetProductById(int productId);
        Task<IEnumerable<products>> GetAllProductsAsync(int busRegId);
        Task DeleteProductAsync(int productId);
      //  Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile);
        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);

      
        List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId);
        bool UpdateProduct(products updatedProduct);
      //  Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);
        IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);
    }
}
