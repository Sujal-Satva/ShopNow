
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

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest model)
        {

            var response = await _authService.RefreshToken(model);
            if (response.Status == 200)
                return Ok(response);
            else if (response.Status == 401)
                return Unauthorized(response);
            else if (response.Status == 404)
                return NotFound(response);
            else
                return StatusCode(500, response);
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest model)
        {
            var response =await _authService.RevokeRefreshToken(model);
            if (response.Status!=200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
