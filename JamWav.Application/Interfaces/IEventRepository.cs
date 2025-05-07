using JamWav.Domain.Entities;

namespace JamWav.Application.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(Guid id);
    Task AddAsync(Event newEvent);
    Task<bool> EventNameExists(string name);
}