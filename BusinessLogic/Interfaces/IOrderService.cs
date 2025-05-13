using SharedModels.Models.Order;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IOrderService
    {
        Task<CommonResponse<string>> AddOrder(Order order);

        Task<CommonResponse<string>> UpdateOrderStatus(string id, string status);
    }
}
