using BusinessLogic.Helper;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using SharedModels.Models;
using SharedModels.Models.Order;
using SharedModels.Models.Seller;


namespace BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        public readonly IOrderRepository _orderRepository;
        public readonly EmailService _emailService;
        public OrderService(IOrderRepository orderRepository,EmailService emailService)
        {
            _orderRepository = orderRepository;
            _emailService = emailService;
        }


        //public async Task<CommonResponse<PagedResponse<ProductModel>>> GetProducts(int page = 1,
        //    int pageSize = 10,
        //    string? search = null,
        //    string? sortColumn = "UpdatedAt",
        //    string? sortDirection = "DESC",
        //    bool isActive = true,
        //    string sellerId = null)
        //{
        //    try
        //    {
        //        var result = await _productRepository.GetProductsFromSPAsync(page, pageSize, search, sortColumn, sortDirection, isActive, sellerId);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new CommonResponse<PagedResponse<ProductModel>>(500, "Product Not Fechted", null);
        //    }
        //}

        public async Task<CommonResponse<string>> AddOrder(Order order)
        {
            try
            {
                await _orderRepository.AddOrderWithItemsAsync(order);
                return new CommonResponse<string>(200, "order added successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<CommonResponse<string>> UpdateOrderStatus(string id, string status)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderStatus(id, status);

                if (result > 0)
                {
                    var customerEmail = _orderRepository.GetCustomerEmailByOrderIdAsync(id);
                    if (customerEmail != null)
                    {
                        var (subject, body) = EmailTemplateService.GetOrderStatusUpdateEmail(id, status);
                        await _emailService.SendEmailAsync(customerEmail.ToString(), subject, body);
                    }
                    return new CommonResponse<string>(200, "Order status updated successfully", null);
                }

                return new CommonResponse<string>(404, "Order not found or not updated", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }


        //public async Task<CommonResponse<string>> UpdateProduct(ProductUpdateModel updateModel)
        //{
        //    try
        //    {
        //        var existingProduct = await _productRepository.GetProductByIdAsync(updateModel.ProductId);
        //        if (existingProduct == null)
        //            return new CommonResponse<string>(404, "Product not found", null);


        //        if (updateModel.PhotoFile != null && updateModel.PhotoFile.Length > 0)
        //        {
        //            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
        //            {
        //                ImageHelper.DeleteImage(existingProduct.ImageUrl);
        //            }

        //            // Upload new image
        //            var newFileName = await ImageHelper.UploadImageAsync(updateModel.PhotoFile);
        //            existingProduct.ImageUrl = newFileName;
        //        }
        //        existingProduct.Name = updateModel.Name;
        //        existingProduct.Description = updateModel.Description;
        //        existingProduct.Price = updateModel.Price;
        //        existingProduct.StockQuantity = updateModel.StockQuantity;
        //        existingProduct.UpdatedAt = DateTime.UtcNow;
        //        existingProduct.IsActive = updateModel.IsActive;

        //        await _productRepository.UpdateProductAsync(existingProduct);

        //        return new CommonResponse<string>(200, "Product updated successfully", null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
        //    }
        //}

        //public async Task<CommonResponse<string>> DeleteProduct(Guid productId, string status)
        //{
        //    try
        //    {
        //        var existingProduct = await _productRepository.GetProductByIdAsync(productId);
        //        if (existingProduct == null)
        //            return new CommonResponse<string>(404, "Product not found", null);
        //        await _productRepository.UpdateProductStatusAsync(productId, bool.Parse(status));

        //        return new CommonResponse<string>(200, "Product status updated successfully", null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
        //    }
        //}


    }
}
