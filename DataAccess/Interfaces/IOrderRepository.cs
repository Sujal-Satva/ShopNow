using SharedModels.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> AddOrderWithItemsAsync(Order order);

        Task<int> UpdateOrderStatus(string id, string status);


        Task<string?> GetCustomerEmailByOrderIdAsync(string orderId);
    }
}
