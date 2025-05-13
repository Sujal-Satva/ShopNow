using SharedModels.Models;
using SharedModels.Models.Auth;
using SharedModels.Models.Seller;


namespace DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task AddSellerDetailsAsync(string userId, SellerDetails seller);

        Task<int> UpdateSellerStatus(string sellerId, int status);

        Task<string> GetSellerEmailById(string sellerId);

        Task<CommonResponse<PagedResponse<ProductModel>>> GetProductsFromSPAsync(
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? sortColumn = "UpdatedAt",
                string? sortDirection = "DESC",
                bool isActive = true,
                string sellerId = null);

        Task AddProductAsync(ProductModel product);

        Task UpdateProductStatusAsync(Guid productId, bool isActive);

        Task<int> UpdateProductAsync(ProductModel product);

        Task<ProductModel> GetProductByIdAsync(Guid productId);
    }
}
