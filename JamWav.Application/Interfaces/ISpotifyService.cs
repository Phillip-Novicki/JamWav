using JamWav.Application.Spotify;

namespace JamWav.Application.Interfaces;

public interface ISpotifyService
{
    string BuildAuthorizationUrl(string state);

    Task<SpotifyTokenResult> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default);

    Task<SpotifyTokenResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<SpotifyProfile> GetCurrentUserProfileAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<SpotifyMusicTaste> GetTopMusicAsync(string accessToken, CancellationToken cancellationToken = default);
}
