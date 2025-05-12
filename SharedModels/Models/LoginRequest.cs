using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }
        public string Role { get; set; }
    }
}
