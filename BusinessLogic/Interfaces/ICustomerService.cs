using SharedModels.Models.Customer;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICustomerService
    {
        Task<CommonResponse<PagedResponse<Customer>>> GetCustomers(int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "UpdatedAt",
            string? sortDirection = "DESC",
            bool isActive = true);

        Task<CommonResponse<string>> DeleteCustomer(Guid customerId, string status);

        Task<CommonResponse<string>> UpdateCustomerAsync(Customer customerModel);
    }
}
