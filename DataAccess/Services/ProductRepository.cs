using DataAccess.Data;
using DataAccess.Helper;
    using DataAccess.Interfaces;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using SharedModels.Models;
    using SharedModels.Models.Auth;
using SharedModels.Models.Seller;
using System.Data;

namespace DataAccess.Services
    {
        public class ProductRepository:IProductRepository
        {
            private readonly DbConnectionFactory _dbConnectionFactory;

            public ProductRepository(IConfiguration config, DbConnectionFactory dbConnectionFactory)
            {
                _dbConnectionFactory = dbConnectionFactory;
            }

            public async Task AddSellerDetailsAsync(string userId, SellerDetails seller)
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var query = @"
                    INSERT INTO Sellers (
                        Id, SellerCompanyName, ProductCategory, City, 
                        IsUniqueCompanyInCity, SellerStatus, DateCreated,
                        ContactPersonName, Email, PhoneNumber, AddressLine, State, PostalCode, Country
                    )
                    VALUES (
                        @Id, @SellerCompanyName, @ProductCategory, @City, 
                        @IsUniqueCompanyInCity, 0, GETDATE(),
                        @ContactPersonName, @Email, @PhoneNumber, @AddressLine,
                         @State, @PostalCode, @Country
                    )";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", userId),
                    new SqlParameter("@SellerCompanyName", seller.SellerCompanyName),
                    new SqlParameter("@ProductCategory", seller.ProductCategory),
                    new SqlParameter("@City", seller.City),
                    new SqlParameter("@IsUniqueCompanyInCity", seller.IsUniqueCompanyInCity),
                    new SqlParameter("@ContactPersonName", seller.ContactPersonName),
                    new SqlParameter("@Email", seller.Email),
                    new SqlParameter("@PhoneNumber", seller.PhoneNumber),
                    new SqlParameter("@AddressLine", seller.AddressLine),
                    new SqlParameter("@State", seller.State ?? (object)DBNull.Value),
                    new SqlParameter("@PostalCode", seller.PostalCode ?? (object)DBNull.Value),
                    new SqlParameter("@Country", seller.Country ?? (object)DBNull.Value)
                };

                await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
            }


            public async Task<int> UpdateSellerStatus(string sellerId, int status)
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var query = @"
                UPDATE Sellers
                SET SellerStatus = @SellerStatus, 
                    DateApproved = CASE WHEN @SellerStatus = 1 THEN GETDATE() ELSE DateApproved END,
                    DateRejected = CASE WHEN @SellerStatus = -1 THEN GETDATE() ELSE DateRejected END
                WHERE Id = @Id";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", sellerId),
                        new SqlParameter("@SellerStatus", status)
                    };

                var rowsAffected = await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
                return rowsAffected;
            }

            public async Task<string> GetSellerEmailById(string sellerId)
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                var query = "SELECT Email FROM Sellers WHERE Id = @Id";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", sellerId)
                };
                var email = await AdoHelper.ExecuteScalarAsync(connection, query, parameters);
                return email.ToString();
            }

                public async Task AddProductAsync(ProductModel product)
                {
                    using var connection = _dbConnectionFactory.CreateConnection();
                    var query = @"
                    INSERT INTO Products (
                        SellerId, Name, Description, Price, StockQuantity,
                        ImageUrl, CreatedAt, IsActive,Category
                    )
                    VALUES (
                        @SellerId, @Name, @Description, @Price, @StockQuantity,
                        @ImageUrl, GETDATE(), @IsActive,@Category
                    )";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@SellerId", product.SellerId),
                        new SqlParameter("@Name", product.Name),
                        new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
                        new SqlParameter("@Price", product.Price),
                        new SqlParameter("@StockQuantity", product.StockQuantity),
                        new SqlParameter("@ImageUrl", product.ImageUrl ?? (object)DBNull.Value),
                        new SqlParameter("@IsActive", "1"),
                        new SqlParameter("@Category",product.Category)
                    };

                    await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
                }

            public async Task<CommonResponse<PagedResponse<ProductModel>>> GetProductsFromSPAsync(
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? sortColumn = "UpdatedAt",
                string? sortDirection = "DESC",
                bool isActive = true,
                string sellerId=null)
            {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                using var command = new SqlCommand("GetPagedProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Page", page);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Search", (object?)search ?? DBNull.Value);
                command.Parameters.AddWithValue("@SortColumn", sortColumn);
                command.Parameters.AddWithValue("@SortDirection", sortDirection);
                command.Parameters.AddWithValue("@IsActive", isActive);
                command.Parameters.AddWithValue("@SellerId", sellerId);

                await connection.OpenAsync();

                var products = new List<ProductModel>();
                int totalCount = 0;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var productModel = new ProductModel
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                            Category=reader.GetString(reader.GetOrdinal("Category")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            SellerId = reader.IsDBNull(reader.GetOrdinal("SellerId")) ? null : reader.GetString(reader.GetOrdinal("SellerId"))
                        };

                        products.Add(productModel);

                    }
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        totalCount = reader.GetInt32(0);
                    }
                }

                var paged = new PagedResponse<ProductModel>(products, page, pageSize, totalCount);
                return new CommonResponse<PagedResponse<ProductModel>>(200, "Products fetched successfully", paged);
            }
            catch (Exception ex)
            {
                return new CommonResponse<PagedResponse<ProductModel>>(500, $"Error fetching products: {ex.Message}", null);
            }
        }


        public async Task<int> UpdateProductAsync(ProductModel product)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
                var query = @"
                UPDATE Products
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    StockQuantity = @StockQuantity,
                    ImageUrl = @ImageUrl,
                    UpdatedAt = GETDATE(),
                    Category=@Category
                WHERE ProductId = @ProductId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductId", product.ProductId),
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@StockQuantity", product.StockQuantity),
                new SqlParameter("@ImageUrl", product.ImageUrl ?? (object)DBNull.Value),
                new SqlParameter("@Category",product.Category )
            };

            return await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
        }

        public async Task UpdateProductStatusAsync(Guid productId, bool isActive)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var query = "UPDATE Products SET IsActive = @IsActive, UpdatedAt = @UpdatedAt WHERE ProductId = @ProductId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@UpdatedAt", DateTime.UtcNow),
                new SqlParameter("@ProductId", productId)
            };

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddRange(parameters.ToArray());

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }


        public async Task<ProductModel> GetProductByIdAsync(Guid productId)
        {
                using var connection = _dbConnectionFactory.CreateConnection();
                var query = "SELECT * FROM Products WHERE ProductId = @ProductId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProductId", productId)
                };

            var result = await AdoHelper.ExecuteReaderListAsync<ProductModel>(connection, query, parameters);
            return result.FirstOrDefault();
        }

    }

}
