
using SharedModels.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using SharedModels.Models.Auth;

namespace ShopNow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var response =await _authService.LoginUser(model);
            if (response.Status != 200)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RegisterRequest model,string roleName) {
            var response = await _authService.CreateUser(model, roleName);
            if (response.Status != 200)
                return BadRequest(response);
            return Ok(response);
        }

        //[HttpPost("refresh")]
        //public IActionResult Refresh([FromBody] RefreshRequest model)
        //{
        //    var response = _authService.ValidateRefreshToken(model.UserId, model.RefreshToken);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    var newRefreshToken = _authService.GenerateRefreshToken();
        //    var saveResponse = _authService.SaveRefreshToken(model.UserId, newRefreshToken);

        //    return Ok(saveResponse);
        //}

        //[HttpPost("revoke-refresh-token")]
        //public IActionResult RevokeRefreshToken([FromBody] RevokeRefreshRequest model)
        //{
        //    var response = _authService.RevokeRefreshToken(model.RefreshToken);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}
    }
}
