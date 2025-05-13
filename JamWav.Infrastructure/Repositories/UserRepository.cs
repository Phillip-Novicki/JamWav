// JamWav.Infrastructure/Repositories/UserRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;          // ApplicationUser lives here
using JamWav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JamWavDbContext _context;
        public UserRepository(JamWavDbContext context)
            => _context = context;

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
            => await _context.Users.FindAsync(id);

        public async Task AddAsync(ApplicationUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
            => await _context.Users.AnyAsync(u => u.UserName == username);
    }
}