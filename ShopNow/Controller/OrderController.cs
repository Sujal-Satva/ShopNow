using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models.Order;
using SharedModels.Models.Seller;

namespace ShopNow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Seller")]
    public class OrderController : ControllerBase
    {
        
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //[HttpGet("products")]
        //public async Task<IActionResult> GetProducts(
        //    int page = 1,
        //    int pageSize = 10,
        //    string? search = null,
        //    string? sortColumn = "UpdatedAt",
        //    string? sortDirection = "DESC",
        //    bool isActive = true,
        //    string? sellerId = null)
        //{
        //    var response = await _productService.GetProducts(page, pageSize, search, sortColumn, sortDirection, isActive, sellerId);

        //    if (response.Status != 200)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        [HttpPost("create-order")]
        public async Task<IActionResult> AddOrder([FromBody] Order order)
        {
            var response = await _orderService.AddOrder(order);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        //[HttpPut("edit-product")]
        //public async Task<IActionResult> EditProduct([FromBody] ProductUpdateModel product)
        //{
        //    var response = await _productService.UpdateProduct(product);

        //    if (response.Status != 200)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        [HttpDelete("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] string orderId, [FromQuery] string status)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, status);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
