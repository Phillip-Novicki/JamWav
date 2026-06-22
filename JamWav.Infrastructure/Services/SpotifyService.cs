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

    public async Task<SpotifyTokenResult> RefreshAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var clientId = _configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("ClientId not configured.");
        var clientSecret = _configuration["Spotify:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not configured.");
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        Dictionary<string, string> authBody = new Dictionary<string, string>();
        
        authBody.Add("grant_type", "refresh_token");
        authBody.Add("refresh_token", refreshToken);
        
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

    public async Task<SpotifyProfile> GetCurrentUserProfileAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadFromJsonAsync<SpotifyProfileResponse>(cancellationToken: cancellationToken);
        var payload = content ?? throw new InvalidOperationException("SpotifyProfile is null.");
        
        return new SpotifyProfile(payload.Id, payload.DisplayName);
    }

    public async Task<SpotifyMusicTaste> GetTopMusicAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var topArtistRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/artists");
        topArtistRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var topArtistResponse = await _httpClient.SendAsync(topArtistRequest, cancellationToken);
        topArtistResponse.EnsureSuccessStatusCode();
        
        var topTracksRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/tracks");
        topTracksRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var topTracksResponse = await _httpClient.SendAsync(topTracksRequest, cancellationToken);
        topTracksResponse.EnsureSuccessStatusCode();
        
        var topArtistContent = await topArtistResponse.Content.ReadFromJsonAsync<SpotifyTopArtistsResponse>(cancellationToken: cancellationToken);
        var topTracksContent = await topTracksResponse.Content.ReadFromJsonAsync<SpotifyTopTracksResponse>(cancellationToken: cancellationToken);
        var artistPayload = topArtistContent ?? throw new InvalidOperationException("SpotifyMusicTaste is null.");
        var tracksPayload = topTracksContent ?? throw new InvalidOperationException("SpotifyMusicTaste is null.");

        var topArtists = artistPayload.Items.Select(a => a.Name).ToList();
        var topTracks = tracksPayload.Items.Select(t => t.Name).ToList();
        var topGenres = artistPayload.Items.SelectMany(a => a.Genres).Distinct().ToList();
        return new SpotifyMusicTaste(topArtists, topTracks, topGenres);
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
        [property: JsonPropertyName("scope")] string Scope
    );
    private record SpotifyProfileResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("display_name")]
        string? DisplayName,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("country")]
        string Country
    );

    private record SpotifyTopArtistsResponse(
        [property: JsonPropertyName("items")] List<SpotifyArtistItem> Items
    );

    private record SpotifyArtistItem(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("genres")] string[] Genres
    );

    private record SpotifyTopTracksResponse(
        [property: JsonPropertyName("items")] List<SpotifyTrackItemResponse> Items
    );

    private record SpotifyTrackItemResponse(
        [property: JsonPropertyName("name")] string Name
    );
}

    