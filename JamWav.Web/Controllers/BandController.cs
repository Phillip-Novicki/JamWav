using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Web.Mapping;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JamWav.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class BandController : ControllerBase
{
    private readonly IBandRepository _bandRepository;

    public BandController(IBandRepository bandRepository)
    {
        _bandRepository = bandRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBands()
    {
        var bands = await _bandRepository.GetAllAsync();
        var responses = bands.Select(b => new BandResponse());
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
            return BadRequest($"0Band `{request.Name}` already exists");
        
        var band = request.ToEntity();
        await _bandRepository.AddAsync(band);
        
        return CreatedAtAction(nameof(GetBandById), new { id = band.Id }, band);
    }
}
