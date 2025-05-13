using JamWav.Domain.Entities;      // brings in ApplicationUser
using JamWav.Web.Models;           // your DTOs

namespace JamWav.Web.Mapping
{
    public static class UserMapper
    {
        // DOMAIN → DTO
        public static UserResponse ToResponse(this ApplicationUser u)
            => new UserResponse
            {
                Id          = u.Id,
                Username    = u.UserName!,
                Email       = u.Email!,
                DisplayName = u.DisplayName,
                CreatedAt   = u.CreatedAt
            };

        // DTO → DOMAIN 
        public static ApplicationUser ToEntity(this CreateUserRequest r)
            => new ApplicationUser
            {
                UserName    = r.Username,
                Email       = r.Email,
                DisplayName = r.DisplayName,
                CreatedAt   = DateTime.UtcNow
            };
    }
}