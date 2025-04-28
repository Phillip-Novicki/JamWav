using JamWav.Application.Interfaces;
using JamWav.Web.Mapping;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JamWav.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class BandsController : ControllerBase
{
    private readonly IBandRepository _bandRepository;

    public BandsController(IBandRepository bandRepository)
    {
        _bandRepository = bandRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBands()
    {
        var bands = await _bandRepository.GetAllAsync();
        var responses = bands.Select(b => b.ToResponse());
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBandById(Guid id)
    {
        var band = await _bandRepository.GetBandById(id);
        if (band is null)
        {
            return NotFound();
        }
        
        return Ok(band.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> CreateBand(CreateBandRequest request)
    {
        if (await _bandRepository.NameExistsAsync(request.Name))
            return BadRequest($"Band `{request.Name}` already exists");
        
        var band = request.ToEntity();
        await _bandRepository.AddAsync(band);
        
        return CreatedAtAction(nameof(GetBandById), new { id = band.Id }, band.ToResponse());
    }
}
