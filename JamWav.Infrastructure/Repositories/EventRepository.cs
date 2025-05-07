using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly JamWavDbContext _context;

    public EventRepository(JamWavDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task AddAsync(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EventNameExists(string title)
    {
        return await _context.Events.AnyAsync(e => e.Title == title);
    }
}