using BusinessLogic.Helper;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using SharedModels.Models;
using SharedModels.Models.Auth;


namespace BusinessLogic.Services
{
    public class AuthService :IAuthService
    {
        public readonly IAuthRepository _authRepository;
        public readonly ITokenService _tokenService;
        public readonly EmailService _emailService;
        public readonly IConfiguration _config;
        public readonly ICustomerRepository _customerRepository;
        public readonly string _adminEmail;
        public readonly IProductRepository _productRepository;

        public AuthService(IAuthRepository authRepository, IProductRepository productRepository, ICustomerRepository customerRepository, IConfiguration configuration, EmailService emailService, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _config = configuration;
            _adminEmail = _config["AdminEmail"];
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<CommonResponse<LoginResponse>> LoginUser(LoginRequest request)
        {
            try
            {
                var result = await _authRepository.VerifyUser(request.Username, request.Password);
                if (result.Status != 200)
                    return new CommonResponse<LoginResponse>(result.Status, result.Message, null);

                var user = result.Data;
                var token = _tokenService.GenerateAccessToken(user.Id, user.RoleName);
                var refreshToken = _tokenService.GenerateRefreshToken();
                _authRepository.SaveRefreshToken(user.Id, refreshToken);

                return new CommonResponse<LoginResponse>(200, "Login successful", new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Username = user.UserName,
                    Role = user.RoleName
                });
            }
            catch(Exception ex)
            {
                return new CommonResponse<LoginResponse>(500, "An error occurred while processing the login request", null);
            }
        }

        public async Task<CommonResponse<LoginResponse>> CreateUser(RegisterRequest request, string roleName)
        {
            try
            {
                if(request.SellerDetails==null && request.CustomerDetails == null)
                {
                    return new CommonResponse<LoginResponse>(400, "SellerDetails or CustomerDetails must be provided", null);
                }
                var result = await _authRepository.CreateUserAsync(request, roleName);
                if (result.Status != 201)
                    return new CommonResponse<LoginResponse>(result.Status, result.Message, null);

                var user = result.Data;
                if (roleName.Equals("Customer", StringComparison.OrdinalIgnoreCase) && request.CustomerDetails != null)
                {
                    await _customerRepository.AddCustomerDetailsAsync(user.Id, request.CustomerDetails);
                    var (subject, body) = EmailTemplateService.GetCustomerWelcomeEmail(request.Username);
                    await _emailService.SendEmailAsync(request.Username, subject, body);
                }
                else if (roleName.Equals("Seller", StringComparison.OrdinalIgnoreCase) && request.SellerDetails != null)
                {
                    await _productRepository.AddSellerDetailsAsync(user.Id, request.SellerDetails);
                    var (subject, body) = EmailTemplateService.GetSellerNotificationEmail(request.Username);
                    await _emailService.SendEmailAsync(_adminEmail, subject, body);
                }
                return new CommonResponse<LoginResponse>(200, $"{roleName} created successfully", new LoginResponse
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Role = user.RoleName
                });
            }
            catch (Exception ex)
            {
                return new CommonResponse<LoginResponse>(500, $"An error occurred while creating the user: {ex.Message}", null);
            }
        }

        public async Task<CommonResponse<LoginResponse>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                bool result = await _authRepository.ValidateRefreshToken(request.RefreshToken);
                if (!result)
                    return new CommonResponse<LoginResponse>(401, "Invalid or expired refresh token", null);

                await _authRepository.RevokeRefreshToken(request.RefreshToken);

                var newRefreshToken = _tokenService.GenerateRefreshToken();
                _authRepository.SaveRefreshToken(request.UserId, newRefreshToken);
                var user = await _authRepository.GetUserById(request.UserId);
                if (user == null)
                    return new CommonResponse<LoginResponse>(404, "User not found", null);
                var token = _tokenService.GenerateAccessToken(user.Id, user.RoleName);

                return new CommonResponse<LoginResponse>(200, "Token refreshed successfully", new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = newRefreshToken,
                    UserId = user.Id,
                    Username = user.UserName,
                    Role = user.RoleName
                });
            }
            catch (Exception ex)
            {
                return new CommonResponse<LoginResponse>(500, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<CommonResponse<string>> RevokeRefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var result = await _authRepository.RevokeRefreshToken(request.RefreshToken);
                if (!result)
                    return new CommonResponse<string>(404, "Refresh token not found", null);
                return new CommonResponse<string>(200, "Refresh token revoked successfully", null);

            }
            catch(Exception ex)
            {
                return new CommonResponse<string>(500, $"An error occurred while revoking the refresh token: {ex.Message}", null);
            }
        }

    }
}
