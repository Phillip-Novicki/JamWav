using JamWav.Domain.Entities;

namespace JamWav.Application.Interfaces;

public interface IBandRepository
{
    Task<Band?> GetBandById(Guid id);
    Task<IEnumerable<Band>> GetAllAsync();
    Task AddAsync(Band band);
    Task<bool> NameExistsAsync(string name);
    
    Task UpdateAsync(Band band);
    Task<bool> DeleteAsync(Guid id);

}