using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Repositories;

public class BandRepository : IBandRepository
{
    private readonly JamWavDbContext _context;

    public BandRepository(JamWavDbContext context)
    {
        _context = context;
    }
    
    public async Task<Band?> GetBandById(Guid id)
    {
        return await _context.Bands.FindAsync(id);
    }

    public async Task<IEnumerable<Band>> GetAllAsync()
    {
        return await _context.Bands.ToListAsync();
    }

    public async Task AddAsync(Band band)
    {
        _context.Bands.Add(band);
        await _context.SaveChangesAsync();
        
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Bands.AnyAsync(b => b.Name == name);
    }
}