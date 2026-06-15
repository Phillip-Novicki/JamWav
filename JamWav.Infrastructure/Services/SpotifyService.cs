using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using JamWav.Application.Interfaces;
using JamWav.Application.Spotify;
using Microsoft.Extensions.Configuration;

namespace JamWav.Infrastructure.Services;

public class SpotifyService : ISpotifyService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private const string ResponseType = "code";
    private const string Scope = "user-read-email user-read-private user-top-read";


    public SpotifyService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public string BuildAuthorizationUrl(string state)
    {

        Dictionary<string, string> authParams = new Dictionary<string, string>();
        authParams.Add("client_id",
            _configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("ClientId not configured."));
        authParams.Add("response_type", ResponseType);
        authParams.Add("redirect_uri",
            _configuration["Spotify:RedirectUri"] ??
            throw new InvalidOperationException("RedirectUri not configured."));
        authParams.Add("scope", Scope);
        authParams.Add("state", state);

        var queryString = string.Join("&", authParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        return $"https://accounts.spotify.com/authorize?{queryString}";
    }

    public async Task<SpotifyTokenResult> ExchangeCodeForTokenAsync(string code,
        CancellationToken cancellationToken = default)
    {
        var clientId = _configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("ClientId not configured.");
        var clientSecret = _configuration["Spotify:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not configured.");
        var redirectUri = _configuration["Spotify:RedirectUri"] ?? throw new InvalidOperationException("RedirectUri not configured.");
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        Dictionary<string, string> authBody = new Dictionary<string, string>();
        authBody.Add("grant_type", "authorization_code");
        authBody.Add("code", code);
        authBody.Add("redirect_uri", redirectUri);
        
        request.Content = new FormUrlEncodedContent(authBody);
        
        string client = $"{clientId}:{clientSecret}";
        var bytes = Encoding.UTF8.GetBytes(client);
        var base64String = Convert.ToBase64String(bytes);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SpotifyTokenResponse>(cancellationToken: cancellationToken);
        var payload = content ?? throw new InvalidOperationException("SpotifyTokenResponse is null.");

        DateTime expiresAt = DateTime.UtcNow.AddSeconds(payload.ExpiresIn);
        return new SpotifyTokenResult(payload.AccessToken, payload.RefreshToken, expiresAt);

    }

    public Task<SpotifyTokenResult> RefreshAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SpotifyProfile> GetCurrentUserProfileAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SpotifyMusicTaste> GetTopMusicAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    private record SpotifyTokenResponse(
        [property: JsonPropertyName("access_token")]
        string AccessToken,
        [property: JsonPropertyName("token_type")]
        string TokenType,
        [property: JsonPropertyName("expires_in")]
        int ExpiresIn,
        [property: JsonPropertyName("refresh_token")]
        string RefreshToken,
        [property: JsonPropertyName("scope")] string scope
    );
}

    