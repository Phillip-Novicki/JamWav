using JamWav.Domain.Entities;

namespace JamWav.Application.Interfaces;

public interface IUserMusicProfileRepository
{
    Task<UserMusicProfile?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserMusicProfile userMusicProfile);
    Task UpdateAsync(UserMusicProfile userMusicProfile);
}