using DataAccess.Interfaces;
using SharedModels.Models.Seller;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using SharedModels.Models.Customer;
using DataAccess.Services;

namespace BusinessLogic.Services
{
    public class CustomerService:ICustomerService
    {
        public readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }


        public async Task<CommonResponse<PagedResponse<Customer>>> GetCustomers(int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "UpdatedAt",
            string? sortDirection = "DESC",
            bool isActive = true)
        {
            try
            {
                var result = await _customerRepository.GetCustomersFromSPAsync(page, pageSize, search, sortColumn, sortDirection, isActive);
                return result;
            }
            catch (Exception ex)
            {
                return new CommonResponse<PagedResponse<Customer>>(500, "Customer Not Fechted", null);
            }
        }

        public async Task<CommonResponse<string>> DeleteCustomer(Guid customerId, string status)
        {
            try
            {
                var existingProduct = await _customerRepository.GetCustomerByIdAsync(customerId);
                if (existingProduct == null)
                    return new CommonResponse<string>(404, "Product not found", null);
                await _customerRepository.UpdateCustomerStatus(customerId, int.Parse(status));

                return new CommonResponse<string>(200, "Product status updated successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }


        public async Task<CommonResponse<string>> UpdateCustomerAsync(Customer customerModel)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerModel.Id);
                if (existingCustomer == null)
                    return new CommonResponse<string>(404, "Customer not found", null);

                // Update customer fields
                existingCustomer.FirstName = customerModel.FirstName;
                existingCustomer.LastName = customerModel.LastName;
                existingCustomer.Email = customerModel.Email;
                existingCustomer.PhoneNumber = customerModel.PhoneNumber;
                existingCustomer.DateOfBirth = customerModel.DateOfBirth;
                existingCustomer.AddressLine1 = customerModel.AddressLine1;
                existingCustomer.AddressLine2 = customerModel.AddressLine2;
                existingCustomer.City = customerModel.City;
                existingCustomer.State = customerModel.State;
                existingCustomer.PostalCode = customerModel.PostalCode;
                existingCustomer.Country = customerModel.Country;
                existingCustomer.ShippingAddressIsSameAsBilling = customerModel.ShippingAddressIsSameAsBilling;
                existingCustomer.DateLastUpdated = DateTime.UtcNow;
                existingCustomer.IsActive = customerModel.IsActive;

                await _customerRepository.UpdateCustomerDetailsAsync(existingCustomer);

                return new CommonResponse<string>(200, "Customer updated successfully", null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred: {ex.Message}", null);
            }
        }

    }
}
