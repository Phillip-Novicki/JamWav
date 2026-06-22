using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Persistence.Repositories;

public class UserMusicProfileRepository : IUserMusicProfileRepository
{
    private readonly JamWavDbContext _context;

    public UserMusicProfileRepository(JamWavDbContext context)
    {
        _context = context;
    }

    public async Task<UserMusicProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserMusicProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(UserMusicProfile userMusicProfile)
    {
        _context.UserMusicProfiles.Add(userMusicProfile);
        await _context.SaveChangesAsync();
        
    }

    public async Task UpdateAsync(UserMusicProfile userMusicProfile)
    {
        _context.UserMusicProfiles.Update(userMusicProfile);
        await _context.SaveChangesAsync();
    }
}