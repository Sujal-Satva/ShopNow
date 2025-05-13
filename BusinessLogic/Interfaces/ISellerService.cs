using SharedModels.Models.Seller;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProductService
    {
        Task<CommonResponse<PagedResponse<ProductModel>>> GetProducts(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "UpdatedAt",
            string? sortDirection = "DESC",
            bool isActive = true,
            string? sellerId = null);

        Task<CommonResponse<string>> AddProduct(ProductCreateModel product);

        Task<CommonResponse<string>> UpdateProduct(ProductUpdateModel updateModel);

        Task<CommonResponse<string>> DeleteProduct(Guid productId, string status);
    }
}
