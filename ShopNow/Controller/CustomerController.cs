using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models.Customer;
using SharedModels.Models.Seller;

namespace ShopNow.Controller
{
    public class CustomerController:ControllerBase
    {

        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(
             int page = 1,
             int pageSize = 10,
             string? search = null,
             string? sortColumn = "DateLastUpdated",
             string? sortDirection = "DESC",
             bool isActive = true)
        {
            var response = await _customerService.GetCustomers(page, pageSize, search, sortColumn, sortDirection, isActive);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("edit-customer")]
        public async Task<IActionResult> EditCustomer([FromBody] Customer customer)
        {
            var response = await _customerService.UpdateCustomerAsync(customer);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpDelete("delete-customer")]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid customerId, [FromQuery] string status)
        {
            var response = await _customerService.DeleteCustomer(customerId, status);

            if (response.Status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
