using JamWav.Application.Interfaces;
using JamWav.Web.Mapping;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JamWav.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FriendsController : ControllerBase
{
    private readonly IFriendRepository _friendRepository;


    public FriendsController(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFriends()
    {
        var friends = await _friendRepository.GetAllAsync();
        var response = friends.Select(f => f.ToResponse());
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFriendById(Guid id)
    {
        var friend = await _friendRepository.GetByIdAsync(id);
        if (friend == null)
            return NotFound();
            
        return Ok(friend.ToResponse());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateFriend(CreateFriendRequest request)
    {
        var friend = request.ToEntity();
        
        await _friendRepository.AddAsync(friend);
        
        return CreatedAtAction(nameof(GetFriendById), new { id = friend.Id }, friend.ToResponse());
    }
}