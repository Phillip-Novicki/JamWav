using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Repositories;

public class FriendRepository : IFriendRepository
{
    private readonly JamWavDbContext _context;

    public FriendRepository(JamWavDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Friend>> GetAllAsync()
    {
        return await _context.Friends.ToListAsync();
    }

    public async Task<Friend?> GetByIdAsync(Guid id)
    {
        return await _context.Friends.FindAsync(id);
    }

    public async Task AddAsync(Friend friend)
    {
        _context.Friends.Add(friend);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Friends.AnyAsync(f => f.FriendName == username);
    
}