using Azure.Core;
using JamWav.Domain.Entities;
using JamWav.Web.Models;

namespace JamWav.Web.Mapping;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user) => new UserResponse()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        DisplayName = user.DisplayName,
        CreatedAt = user.CreatedAt,
    };

    public static User ToEntity(this CreateUserRequest request) => new User(
        request.Username,
        request.Email,
        request.DisplayName
    );
}