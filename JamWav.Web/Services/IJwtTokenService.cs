using JamWav.Domain.Entities;

namespace JamWav.Web.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}