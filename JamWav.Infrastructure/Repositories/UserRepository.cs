using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JamWavDbContext _context;

    public UserRepository(JamWavDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }
}