using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;
using JamWav.Web.Mapping;

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
        var responses = users.Select(u => u.ToResponse());
        return Ok(responses);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user.ToResponse());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return BadRequest($"Username '{request.Username}' already exists");
        }

        var user = request.ToEntity();

        await _userRepository.AddAsync(user);

        return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user.ToResponse());
    }
}
