using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JamWav.Domain.Entities;
using JamWav.Web.Models;
using JamWav.Web.Services;

namespace JamWav.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwt;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwt)
        {
            _userManager = userManager;
            _jwt          = jwt;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized();

            var token = _jwt.GenerateToken(user);
            return Ok(new LoginResponse { Token = token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            if (await _userManager.FindByNameAsync(dto.Username) != null)
                return BadRequest($"Username '{dto.Username}' already exists");

            var user = new ApplicationUser
            {
                UserName    = dto.Username,
                Email       = dto.Email,
                DisplayName = dto.Username
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // map to your response model:
            var response = new UserResponse
            {
                Username    = user.UserName!,
                Email       = user.Email!,
                DisplayName = user.DisplayName!
            };

            return CreatedAtAction(nameof(Register), new { username = user.UserName }, response);
        }
    }
}
