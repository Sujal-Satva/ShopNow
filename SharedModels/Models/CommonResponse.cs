using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class CommonResponse<T>
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public CommonResponse(int? status, string? message = null, T? data = default)
        {
            Status = status ?? 0;
            Message = message;
            Data = data;
        }

        public CommonResponse()
        {
        }
    }
}
