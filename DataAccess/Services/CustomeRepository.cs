using DataAccess.Data;
using DataAccess.Helper;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SharedModels.Models;
using SharedModels.Models.Customer;
using SharedModels.Models.Auth;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SharedModels.Models.Seller;

namespace DataAccess.Services
{
    public class CustomerRepository :ICustomerRepository
    {
        private readonly IConfiguration _config;
        private readonly DbConnectionFactory _dbConnectionFactory;

        public CustomerRepository(IConfiguration config, DbConnectionFactory dbConnectionFactory)
        {
            _config = config;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task AddCustomerDetailsAsync(string userId, CustomerDetails customer)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var query = @"
                INSERT INTO Customers (
                    Id, FirstName, LastName, Email, PhoneNumber, DateOfBirth,
                    AddressLine1, AddressLine2, City, State, PostalCode, Country,
                    ShippingAddressIsSameAsBilling
                )
                VALUES (
                    @Id, @FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth,
                    @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country,
                    @ShippingAddressIsSameAsBilling
                )";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userId),
                new SqlParameter("@FirstName", customer.FirstName),
                new SqlParameter("@LastName", customer.LastName),
                new SqlParameter("@Email", customer.Email),
                new SqlParameter("@PhoneNumber", customer.PhoneNumber),
                new SqlParameter("@DateOfBirth", customer.DateOfBirth),
                new SqlParameter("@AddressLine1", customer.AddressLine1),
                new SqlParameter("@AddressLine2", customer.AddressLine2 ?? (object)DBNull.Value),
                new SqlParameter("@City", customer.City),
                new SqlParameter("@State", customer.State ?? (object)DBNull.Value),
                new SqlParameter("@PostalCode", customer.PostalCode ?? (object)DBNull.Value),
                new SqlParameter("@Country", customer.Country ?? (object)DBNull.Value),
                new SqlParameter("@ShippingAddressIsSameAsBilling", customer.ShippingAddressIsSameAsBilling)
            };

            await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
        }


        public async Task<int> UpdateCustomerStatus(Guid customerId, int status)
        {

            var query = "UPDATE Customers SET IsActive = @IsActive, DateLastUpdated = @DateLastUpdated WHERE CustomerId = @CustomerId";
            using var connection = _dbConnectionFactory.CreateConnection();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@IsActive", status),
                new SqlParameter("@DateLastUpdated", DateTime.UtcNow),
                new SqlParameter("@CustomerId", customerId)
            };

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddRange(parameters.ToArray());

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
            var rowsAffected = await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
            return rowsAffected;
        }

        public async Task<string> GetCustomerEmailById(Guid customerId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var query = "SELECT Email FROM Customers WHERE Id = @Id";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", customerId)
            };
            var email = await AdoHelper.ExecuteScalarAsync(connection, query, parameters);
            return email.ToString();
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid customerId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var query = "SELECT * FROM Customers WHERE Id = @ProductId";

            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerId)
                };

            var result = await AdoHelper.ExecuteReaderListAsync<Customer>(connection, query, parameters);
            return result.FirstOrDefault();
        }


        public async Task<int> UpdateCustomerDetailsAsync(Customer customer)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var query = @"
                UPDATE Customers
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    PhoneNumber = @PhoneNumber,
                    DateOfBirth = @DateOfBirth,
                    AddressLine1 = @AddressLine1,
                    AddressLine2 = @AddressLine2,
                    City = @City,
                    State = @State,
                    PostalCode = @PostalCode,
                    Country = @Country,
                    ShippingAddressIsSameAsBilling = @ShippingAddressIsSameAsBilling,
                    DateLastUpdated = GETUTCDATE()
                WHERE Id = @Id";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", customer.Id),
                new SqlParameter("@FirstName", customer.FirstName),
                new SqlParameter("@LastName", customer.LastName),
                new SqlParameter("@PhoneNumber", (object?)customer.PhoneNumber ?? DBNull.Value),
                new SqlParameter("@DateOfBirth", customer.DateOfBirth),
                new SqlParameter("@AddressLine1", customer.AddressLine1),
                new SqlParameter("@AddressLine2", (object?)customer.AddressLine2 ?? DBNull.Value),
                new SqlParameter("@City", customer.City),
                new SqlParameter("@State", (object?)customer.State ?? DBNull.Value),
                new SqlParameter("@PostalCode", (object?)customer.PostalCode ?? DBNull.Value),
                new SqlParameter("@Country", (object?)customer.Country ?? DBNull.Value),
                new SqlParameter("@ShippingAddressIsSameAsBilling", customer.ShippingAddressIsSameAsBilling)
            };

            return await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
        }



        public async Task<CommonResponse<PagedResponse<Customer>>> GetCustomersFromSPAsync(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "DateLastUpdated",
            string? sortDirection = "DESC",
            bool isActive = true)
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                using var command = new SqlCommand("GetPagedCustomers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Page", page);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Search", (object?)search ?? DBNull.Value);
                command.Parameters.AddWithValue("@SortColumn", sortColumn);
                command.Parameters.AddWithValue("@SortDirection", sortDirection);
                command.Parameters.AddWithValue("@IsActive", isActive);

                await connection.OpenAsync();

                var customers = new List<Customer>();
                int totalCount = 0;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var customer = new Customer
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                            AddressLine2 = reader.GetString(reader.GetOrdinal("AddressLine2")),
                            City = reader.GetString(reader.GetOrdinal("City")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            ShippingAddressIsSameAsBilling = reader.GetBoolean(reader.GetOrdinal("ShippingAddressIsSameAsBilling")),
                            DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                            DateLastUpdated = reader.IsDBNull(reader.GetOrdinal("DateLastUpdated")) ? null : reader.GetDateTime(reader.GetOrdinal("DateLastUpdated")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };

                        customers.Add(customer);
                    }

                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        totalCount = reader.GetInt32(0);
                    }
                }

                var paged = new PagedResponse<Customer>(customers, page, pageSize, totalCount);
                return new CommonResponse<PagedResponse<Customer>>(200, "Customers fetched successfully", paged);
            }
            catch (Exception ex)
            {
                return new CommonResponse<PagedResponse<Customer>>(500, $"Error fetching customers: {ex.Message}", null);
            }
        }

    }

}
