    using DataAccess.Data;
using DataAccess.Helper;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
using SharedModels.Models;
using SharedModels.Models.Auth;

namespace DataAccess.Services
    {
        public class AuthRepository : IAuthRepository
        {
            private readonly IConfiguration _config;
            private readonly DbConnectionFactory _dbConnectionFactory;

            public AuthRepository(IConfiguration config, DbConnectionFactory dbConnectionFactory)
            {
                _config = config;
                _dbConnectionFactory = dbConnectionFactory;
            }

        public async Task<CommonResponse<UserWithRole>> VerifyUser(string username, string password)
        {
            try
            {
                var query = @"
                    SELECT *, r.Name AS RoleName
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                    WHERE u.UserName = @Username";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Username", username)
                };

                using var connection = _dbConnectionFactory.CreateConnection();
                var users = await AdoHelper.ExecuteReaderListAsync<UserWithRole>(connection, query, parameters);

                if (users.Count == 0)
                    return new CommonResponse<UserWithRole> { Status = 404, Message = "User not found", Data = null };

                var user = users[0];
                var passwordHasher = new PasswordHasher<UserWithRole>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                return result == PasswordVerificationResult.Success
                    ? new CommonResponse<UserWithRole> { Status = 200, Message = "Login successful", Data = user }
                    : new CommonResponse<UserWithRole> { Status = 401, Message = "Invalid username or password", Data = null };
            }
            catch (Exception ex)
            {
                return new CommonResponse<UserWithRole> { Status = 500, Message = $"An error occurred: {ex.Message}", Data = null };
            }
        }

        public async Task<CommonResponse<UserWithRole>> CreateUserAsync(RegisterRequest request, string roleName)
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                var checkQuery = "SELECT COUNT(1) FROM AspNetUsers WHERE UserName = @Username";
                var checkParams = new List<SqlParameter> { new SqlParameter("@Username", request.Username) };
                var exists = (int)(await AdoHelper.ExecuteScalarAsync(connection, checkQuery, checkParams)) > 0;

                if (exists)
                    return new CommonResponse<UserWithRole>(409, "Username already exists", null);
                var userId = Guid.NewGuid().ToString();
                var hasher = new PasswordHasher<UserWithRole>();
                var hashedPassword = hasher.HashPassword(new UserWithRole(), request.Password);
                var insertUserQuery = @"
                INSERT INTO AspNetUsers (Id, UserName, PasswordHash, Email, NormalizedEmail)
                VALUES (@Id, @Username, @PasswordHash, @Email, @NormalizedEmail)";

                var insertUserParams = new List<SqlParameter>
                {
                    new SqlParameter("@Id", userId),
                    new SqlParameter("@Username", request.Username),
                    new SqlParameter("@PasswordHash", hashedPassword),
                    new SqlParameter("@Email", request.Username),
                    new SqlParameter("@NormalizedEmail", request.Username.ToLower()), 
                    
                };

                await AdoHelper.ExecuteNonQueryAsync(connection, insertUserQuery, insertUserParams);
                var roleIdQuery = "SELECT Id FROM AspNetRoles WHERE Name = @RoleName";
                var roleIdParams = new List<SqlParameter> { new SqlParameter("@RoleName", roleName) };
                var roleId = (string)await AdoHelper.ExecuteScalarAsync(connection, roleIdQuery, roleIdParams);


                var assignRoleQuery = @"
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId)";
                var assignRoleParams = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@RoleId", roleId)
                };
                await AdoHelper.ExecuteNonQueryAsync(connection, assignRoleQuery, assignRoleParams);

                var createdUser = new UserWithRole
                {
                    Id = userId,
                    UserName = request.Username,
                };

                return new CommonResponse<UserWithRole>(201, "User created successfully", createdUser);
            }
            catch (Exception ex)
            {
                return new CommonResponse<UserWithRole>(500, $"Error creating user: {ex.Message}", null);
            }
        }

        public async void SaveRefreshToken(string userId, string refreshToken)
        {
            var query = @"
                INSERT INTO RefreshTokens (UserId, Token, CreatedAt, ExpiresAt, IsRevoked) 
                VALUES (@UserId, @Token, @CreatedAt, @ExpiresAt, 0)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Token", refreshToken),
                new SqlParameter("@CreatedAt", DateTime.UtcNow),
                new SqlParameter("@ExpiresAt", DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpiryDays"])))
            };

            using var connection = _dbConnectionFactory.CreateConnection();
            await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
        }

        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            var query = @"
                SELECT COUNT(*) FROM RefreshTokens 
                WHERE Token = @Token AND IsRevoked = 0 AND ExpiresAt > GETUTCDATE()";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Token", refreshToken)
            };

            using var connection = _dbConnectionFactory.CreateConnection();
            var result =await AdoHelper.ExecuteScalarAsync(connection, query, parameters);
            return (int)result > 0;
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            var query = @"UPDATE RefreshTokens SET IsRevoked = 1 WHERE Token = @Token";
            var parameters = new List<SqlParameter> { new SqlParameter("@Token", refreshToken) };

            using var connection = _dbConnectionFactory.CreateConnection();
            var result=await AdoHelper.ExecuteNonQueryAsync(connection, query, parameters);
            return (int)result > 0;
        }

        public async Task<UserWithRole> GetUserById(string userId)
        {
            try
            {
                var query = @"
                    SELECT u.*, r.Name AS RoleName
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                    WHERE u.Id = @UserId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId)
                };

                using var connection = _dbConnectionFactory.CreateConnection();
                var users = await AdoHelper.ExecuteReaderListAsync<UserWithRole>(connection, query, parameters);

                return users.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

    }

}
