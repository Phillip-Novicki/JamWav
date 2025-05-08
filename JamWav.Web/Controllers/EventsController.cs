using JamWav.Application.Interfaces;
using JamWav.Web.Mapping;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JamWav.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repository;

    public EventsController(IEventRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var list = await _repository.GetAllAsync();
        return Ok(list.Select(e => e.ToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var e = await _repository.GetByIdAsync(id);
        if (e is null) return NotFound();
        return Ok(e.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(CreateEventRequest request)
    {
        var e = request.ToEntity();
        await _repository.AddAsync(e);
        return CreatedAtAction(nameof(GetEventById), new { id = e.Id }, e.ToResponse());
    }
}