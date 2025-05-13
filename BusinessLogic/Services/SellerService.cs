using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using SharedModels.Models;
using SharedModels.Models.Seller;


namespace BusinessLogic.Services
{
    public class ProductService:IProductService
    {
        public readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }


        public async Task<CommonResponse<PagedResponse<ProductModel>>> GetProducts(int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "UpdatedAt",
            string? sortDirection = "DESC",
            bool isActive = true,
            string sellerId=null)
        {
            try
            {
                var result=await _productRepository.GetProductsFromSPAsync(page, pageSize, search, sortColumn, sortDirection, isActive,sellerId);
                return result;
            }catch(Exception ex)
            {
                return new CommonResponse<PagedResponse<ProductModel>>(500, "Product Not Fechted", null);
            }
        }

        public async Task<CommonResponse<string>> AddProduct(ProductCreateModel productCreateModel)
        {
            try
            {
                if (productCreateModel.PhotoFile == null || productCreateModel.PhotoFile.Length == 0)
                {
                    return new CommonResponse<string>(400, "No photo provided.", null);
                }
                string fileName = await ImageHelper.UploadImageAsync(productCreateModel.PhotoFile);

                var product = new ProductModel
                { 
                    SellerId = productCreateModel.SellerId,
                    Name = productCreateModel.Name,
                    Description = productCreateModel.Description,
                    Price = productCreateModel.Price,
                    Category=productCreateModel.Category,
                    StockQuantity = productCreateModel.StockQuantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ImageUrl = fileName 
                };
                await _productRepository.AddProductAsync(product);
                return new CommonResponse<string>(200, "Product added successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<CommonResponse<string>> UpdateProduct(ProductUpdateModel updateModel)
        {
            try
            {
                var existingProduct = await _productRepository.GetProductByIdAsync(updateModel.ProductId);
                if (existingProduct == null)
                    return new CommonResponse<string>(404, "Product not found", null);


                if (updateModel.PhotoFile != null && updateModel.PhotoFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        ImageHelper.DeleteImage(existingProduct.ImageUrl);
                    }

                    // Upload new image
                    var newFileName = await ImageHelper.UploadImageAsync(updateModel.PhotoFile);
                    existingProduct.ImageUrl = newFileName;
                }
                existingProduct.Name = updateModel.Name;
                existingProduct.Description = updateModel.Description;
                existingProduct.Price = updateModel.Price;
                existingProduct.StockQuantity = updateModel.StockQuantity;
                existingProduct.UpdatedAt = DateTime.UtcNow;
                existingProduct.IsActive = updateModel.IsActive;

                await _productRepository.UpdateProductAsync(existingProduct);

                return new CommonResponse<string>(200, "Product updated successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<CommonResponse<string>> DeleteProduct(Guid productId, string status)
        {
            try
            {
                var existingProduct = await _productRepository.GetProductByIdAsync(productId);
                if (existingProduct == null)
                    return new CommonResponse<string>(404, "Product not found", null);
                await _productRepository.UpdateProductStatusAsync(productId, bool.Parse(status));

                return new CommonResponse<string>(200, "Product status updated successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }


    }
}
