
using BusinessLogic.Helper;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;

using SharedModels.Models;
using SharedModels.Models.Auth;


namespace BusinessLogic.Services
{
    public class AuthService :IAuthService
    {
        public readonly IAuthRepository _authRepository;
        public readonly ITokenService _tokenService;
        public readonly EmailService _emailService;

        public AuthService(IAuthRepository authRepository,EmailService emailService, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _emailService = emailService;
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
                var result = await _authRepository.CreateUserAsync(request, roleName);

                if (result.Status != 201)
                    return new CommonResponse<LoginResponse>(result.Status, result.Message, null);

                var user = result.Data;
                string subject = string.Empty;
                string body = string.Empty;

                if (roleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    subject = "Welcome to Our Application, Customer";
                    body = $"Dear {request.Username},<br><br>Thank you for registering with us. Your account has been successfully created.";
                    await _emailService.SendEmailAsync(request.Username, subject, body); 
                }
                else if (roleName.Equals("Seller", StringComparison.OrdinalIgnoreCase))
                {
                    subject = "New Seller Registration Pending Approval";
                    body = $"Dear Admin,<br><br>A new seller has registered with the username {request.Username}. Please review and approve or reject this seller's account.";
                    await _emailService.SendEmailAsync("sujalkareliya27@gmail.com", subject, body);
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

    }
}
