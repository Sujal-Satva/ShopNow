using DataAccess.Data;
using DataAccess.Helper;
using DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SharedModels.Models.Order;

namespace DataAccess.Services
{
    public class OrderRepository:IOrderRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public OrderRepository(IConfiguration config, DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> AddOrderWithItemsAsync(Order order)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1: Insert Order and get OrderId
                var insertOrderQuery = @"
                        INSERT INTO Orders (
                            CustomerId, 
                            OrderDate, 
                            TotalAmount, 
                            ShippingAddress, 
                            OrderStatus, 
                            CreatedAt, 
                            UpdatedAt
                        )
                        OUTPUT INSERTED.OrderId
                        VALUES (
                            @CustomerId, 
                            @OrderDate, 
                            @TotalAmount, 
                            @ShippingAddress, 
                            @OrderStatus, 
                            GETDATE(), 
                            GETDATE()
                        )";

                var orderParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", order.CustomerId),
                    new SqlParameter("@OrderDate", order.OrderDate),
                    new SqlParameter("@TotalAmount", order.TotalAmount),
                    new SqlParameter("@ShippingAddress", order.ShippingAddress ?? (object)DBNull.Value),
                    new SqlParameter("@OrderStatus", "0")
                };

                var orderId = (int)(await AdoHelper.ExecuteScalarAsync(connection, insertOrderQuery, orderParameters, transaction));
                foreach (var item in order.Items)
                {
                    var insertItemQuery = @"
                        INSERT INTO OrderItems (
                            OrderId, 
                            ProductId, 
                            Quantity, 
                            UnitPrice, 
                            TotalPrice
                        )
                        VALUES (
                            @OrderId, 
                            @ProductId, 
                            @Quantity, 
                            @UnitPrice, 
                            @TotalPrice
                        )";

                    var itemParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@ProductId", item.ProductId),
                        new SqlParameter("@Quantity", item.Quantity),
                        new SqlParameter("@UnitPrice", item.UnitPrice),
                        new SqlParameter("@TotalPrice", item.TotalPrice)
                    };

                    await AdoHelper.ExecuteNonQueryAsync(connection, insertItemQuery, itemParameters, transaction);
                }
                transaction.Commit();

                return orderId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<string?> GetCustomerEmailByOrderIdAsync(string orderId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var query = @"
                SELECT c.Email
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                WHERE o.OrderId = @OrderId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", Guid.Parse(orderId))
            };

            var result = await AdoHelper.ExecuteScalarAsync(connection, query, parameters);

            return result != null ? result.ToString() : null;
        }



        public async Task<int> UpdateOrderStatus(string id, string status)
        {
           using var connection = _dbConnectionFactory.CreateConnection();

                    var query = @"
                    UPDATE Orders
                    SET OrderStatus = @OrderStatus,
                        UpdatedAt = GETDATE()
                    WHERE OrderId = @OrderId";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@OrderId", id),
                        new SqlParameter("@OrderStatus", status)
                    };

            return await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
        }

    }
}
