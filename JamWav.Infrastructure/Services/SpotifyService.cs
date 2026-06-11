using JamWav.Application.Interfaces;
using JamWav.Application.Spotify;
using Microsoft.Extensions.Configuration;

namespace JamWav.Infrastructure.Services;

public class SpotifyService : ISpotifyService
{
    private readonly IConfiguration _configuration;
    private const string ResponseType = "code";
    private const string Scope = "user-read-email user-read-private user-top-read";


    public SpotifyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BuildAuthorizationUrl(string state)
    {
        
        Dictionary<string, string> authParams = new Dictionary<string, string>();
        authParams.Add("client_id", _configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("ClientId not configured."));
        authParams.Add("response_type", ResponseType);
        authParams.Add("redirect_uri", _configuration["Spotify:RedirectUri"] ?? throw new InvalidOperationException("RedirectUri not configured."));
        authParams.Add("scope", Scope);
        authParams.Add("state", state);
        
        var queryString = string.Join("&", authParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        return $"https://accounts.spotify.com/authorize?{queryString}";
    }

    public Task<SpotifyTokenResult> ExchangeCodeForTokenAsync(string code,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<SpotifyTokenResult> RefreshAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<SpotifyProfile> GetCurrentUserProfileAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<SpotifyMusicTaste> GetTopMusicAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}