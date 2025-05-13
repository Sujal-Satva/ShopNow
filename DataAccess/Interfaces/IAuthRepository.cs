using Microsoft.AspNetCore.Identity;
using SharedModels.Models;
using SharedModels.Models.Auth;

namespace DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        Task<CommonResponse<UserWithRole>> VerifyUser(string username, string password);

        void SaveRefreshToken(string userId, string refreshToken);

        Task<CommonResponse<UserWithRole>> CreateUserAsync(RegisterRequest request, string roleName);

        Task<bool> ValidateRefreshToken(string refreshToken);

        Task<bool> RevokeRefreshToken(string refreshToken);

        Task<UserWithRole> GetUserById(string userId);
    }
}
