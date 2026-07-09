using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ProductApiAssessment.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // Demo Login
            if (username != "admin" || password != "admin123")
            {
                return Unauthorized("Invalid Username or Password");
            }

            var accessToken = _tokenService.GenerateAccessToken(username);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save Refresh Token in memory
            _tokenService.SaveRefreshToken(username, refreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!_tokenService.ValidateRefreshToken(request.RefreshToken))
            {
                return Unauthorized("Invalid Refresh Token");
            }

            var username = _tokenService.GetUsernameFromRefreshToken(request.RefreshToken);

            if (username == null)
            {
                return Unauthorized("Invalid Refresh Token");
            }

            // Remove old refresh token
            _tokenService.RemoveRefreshToken(request.RefreshToken);

            // Generate new tokens
            var accessToken = _tokenService.GenerateAccessToken(username);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Save new refresh token
            _tokenService.SaveRefreshToken(username, newRefreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}