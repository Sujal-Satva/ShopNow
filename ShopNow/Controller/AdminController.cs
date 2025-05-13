using SharedModels.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using SharedModels.Models.Auth;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;

namespace ShopNow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;

        public AdminController(IAdminService adminService, IProductService productService)
        {
            _adminService = adminService;
            _productService = productService;
        }

        [HttpGet("update-seller-status")]
        public async Task<IActionResult> UpdateSellerStatus(string id, string status)
        {
            var response = await _adminService.UpdateSellerStatus(id, status);
            if (response.Status != 200)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("update-product-status")]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid productId, [FromQuery] string status)
        {
            var response = await _productService.DeleteProduct(productId, status);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
