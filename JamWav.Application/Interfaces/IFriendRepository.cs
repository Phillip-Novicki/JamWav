using JamWav.Domain.Entities;

namespace JamWav.Application.Interfaces;

public interface IFriendRepository
{
    Task<IEnumerable<Friend>> GetAllAsync();
    Task<Friend?> GetByIdAsync(Guid id);
    Task AddAsync(Friend friend);
    Task <bool> UsernameExistsAsync(string username);
}

