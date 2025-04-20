using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace JamWav.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return BadRequest($"Username '{request.Username}' already exists");
        }

        var user = new User(
            request.Username,
            request.Email,
            request.DisplayName
        );

        await _userRepository.AddAsync(user);

        return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user);
    }
}
