using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models.Seller;

namespace ShopNow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Seller")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "UpdatedAt",
            string? sortDirection = "DESC",
            bool isActive = true,
            string? sellerId = null)
        {
            var response = await _productService.GetProducts(page, pageSize, search, sortColumn, sortDirection, isActive, sellerId);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateModel product)
        {
            var response = await _productService.AddProduct(product);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("edit-product")]
        public async Task<IActionResult> EditProduct([FromBody] ProductUpdateModel product)
        {
            var response = await _productService.UpdateProduct(product);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid productId, [FromQuery] string status)
        {
            var response = await _productService.DeleteProduct(productId, status);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
