using JamWav.Domain.Entities;
using MediatR;

namespace JamWav.Application.Spotify.Commands.ConnectSpotifyAccount;

public record ConnectSpotifyAccountCommand(Guid UserId, string Code) : IRequest<UserMusicProfile>, IRequest;