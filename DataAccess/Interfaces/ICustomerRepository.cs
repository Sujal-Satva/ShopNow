using SharedModels.Models;
using SharedModels.Models.Auth;
using SharedModels.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomerDetailsAsync(string userId, CustomerDetails customer);

        Task<CommonResponse<PagedResponse<Customer>>> GetCustomersFromSPAsync(
           int page = 1,
           int pageSize = 10,
           string? search = null,
           string? sortColumn = "DateLastUpdated",
           string? sortDirection = "DESC",
           bool isActive = true);

        Task<int> UpdateCustomerStatus(Guid customerId, int status);

        Task<string> GetCustomerEmailById(Guid customerId);

        Task<Customer> GetCustomerByIdAsync(Guid customerId);

        Task<int> UpdateCustomerDetailsAsync(Customer customer);
    }
}
