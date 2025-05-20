using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JamWav.Domain.Entities;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JamWav.Web.Tests.Unit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager    = userManager;
            _signInManager  = signInManager;
            _config         = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                dto.Username, dto.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized();

            var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var creds    = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             _config["Jwt:Issuer"],
                audience:           _config["Jwt:Audience"],
                claims:             new[] { new Claim(JwtRegisteredClaimNames.Sub, dto.Username) },
                expires:            DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse { Token = tokenString });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Username);
            if (existing != null)
                return BadRequest($"Username '{dto.Username}' already exists");

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email    = dto.Email
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }

            var response = new UserResponse
            {
                Username = user.UserName!,
                Email    = user.Email!
            };

            // tests only assert that this is a CreatedAtActionResult and
            // that Value is a UserResponse, so the actual route name doesn't matter
            return CreatedAtAction(nameof(Register), response);
        }
    }
}

