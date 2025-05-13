using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Admin
{
    public class ApproveRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
