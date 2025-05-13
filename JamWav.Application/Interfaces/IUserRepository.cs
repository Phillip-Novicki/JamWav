using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWav.Domain.Entities;

namespace JamWav.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser?> GetByIdAsync(Guid id);
    Task AddAsync(ApplicationUser user);
    Task<bool> UsernameExistsAsync(string username);
}