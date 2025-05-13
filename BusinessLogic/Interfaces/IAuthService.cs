using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Models;
using SharedModels.Models.Auth;

namespace BusinessLogic.Interfaces
{
    public interface IAuthService
    {
        Task<CommonResponse<LoginResponse>> LoginUser(LoginRequest request);

        Task<CommonResponse<LoginResponse>> CreateUser(RegisterRequest request, string roleName);


        Task<CommonResponse<LoginResponse>> RefreshToken(RefreshTokenRequest request);

        Task<CommonResponse<string>> RevokeRefreshToken(RefreshTokenRequest request);


    }
}
