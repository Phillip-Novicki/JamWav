using System.Net;
using System.Text;
using JamWav.Infrastructure.Services;
using JamWav.Infrastructure.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace JamWav.Infrastructure.Tests.Services;

public class SpotifyServiceTests
{
    [Fact]
    public void BuildAuthorizationUrl_ShouldReturnAuthorizationUrl()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["Spotify:ClientId"] = "test-client-id",
            ["Spotify:RedirectUri"] = "https://localhost:7291/api/spotify/callback",
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var sut = new SpotifyService(config, new HttpClient());

        // Act
        var result = sut.BuildAuthorizationUrl("test-state");

        // Assert
        Assert.StartsWith("https://accounts.spotify.com/authorize?", result);
        Assert.Contains("client_id=test-client-id", result);
        Assert.Contains("response_type=code", result);
        Assert.Contains($"redirect_uri={Uri.EscapeDataString("https://localhost:7291/api/spotify/callback")}", result);
        Assert.Contains("scope=user-read-email%20user-read-private%20user-top-read", result);
        Assert.Contains("state=test-state", result);
    }

    [Fact]
    public async Task GetTopMusicAsync_ShouldReturnSpotifyMusicTaste_WhenSpotifyRespondsSuccessfully()
    {
        // Arrange
        const string artistsJson = """
            {
              "items": [
                { "name": "Test Artist One", "genres": ["rock", "indie"] },
                { "name": "Test Artist Two", "genres": ["indie", "alternative"] }
              ]
            }
            """;

        const string tracksJson = """
            {
              "items": [
                { "name": "Test Track One" },
                { "name": "Test Track Two" }
              ]
            }
            """;

        var handler = new FakeHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
        {
            ["https://api.spotify.com/v1/me/top/artists"] = new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(artistsJson, Encoding.UTF8, "application/json") },
            ["https://api.spotify.com/v1/me/top/tracks"] = new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(tracksJson, Encoding.UTF8, "application/json") }
        });
        var sut = new SpotifyService(new ConfigurationBuilder().Build(), new HttpClient(handler));

        // Act
        var result = await sut.GetTopMusicAsync("test-access-token");

        // Assert
        Assert.Equal(new[] { "Test Artist One", "Test Artist Two" }, result.TopArtists);
        Assert.Equal(new[] { "Test Track One", "Test Track Two" }, result.TopTracks);
        Assert.Contains("rock", result.TopGenres);
        Assert.Contains("indie", result.TopGenres);
        Assert.Contains("alternative", result.TopGenres);
        Assert.Single(result.TopGenres, g => g == "indie"); // deduped
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_ShouldReturnSpotifyProfile_WhenSpotifyRespondsSuccessfully()
    {
        // Arrange
        const string responseJson = """
            {
              "id": "test-spotify-user-id",
              "display_name": "Test User",
              "email": "test.user@test.com",
              "country": "US"
            }
            """;

        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });
        var sut = new SpotifyService(new ConfigurationBuilder().Build(), new HttpClient(handler));

        // Act
        var result = await sut.GetCurrentUserProfileAsync("test-access-token");

        // Assert
        Assert.Equal("test-spotify-user-id", result.SpotifyUserId);
        Assert.Equal("Test User", result.DisplayName);
        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("https://api.spotify.com/v1/me", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_ShouldReturnNewTokenResult_WhenSpotifyRespondsSuccessfully()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["Spotify:ClientId"] = "test-client-id",
            ["Spotify:ClientSecret"] = "test-client-secret",
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        const string responseJson = """
            {
              "access_token": "new-access-token",
              "token_type": "Bearer",
              "expires_in": 3600,
              "refresh_token": "new-refresh-token",
              "scope": "user-read-email user-read-private user-top-read"
            }
            """;

        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });
        var sut = new SpotifyService(config, new HttpClient(handler));

        // Act
        var result = await sut.RefreshAccessTokenAsync("old-refresh-token");

        // Assert
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://accounts.spotify.com/api/token", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ShouldReturnTokenResult_WhenSpotifyRespondsSuccessfully()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["Spotify:ClientId"] = "test-client-id",
            ["Spotify:ClientSecret"] = "test-client-secret",
            ["Spotify:RedirectUri"] = "https://localhost:7291/api/spotify/callback",
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        const string responseJson = """
            {
              "access_token": "test-access-token",
              "token_type": "Bearer",
              "expires_in": 3600,
              "refresh_token": "test-refresh-token",
              "scope": "user-read-email user-read-private user-top-read"
            }
            """;

        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });
        var sut = new SpotifyService(config, new HttpClient(handler));

        // Act
        var result = await sut.ExchangeCodeForTokenAsync("test-auth-code");

        // Assert
        Assert.Equal("test-access-token", result.AccessToken);
        Assert.Equal("test-refresh-token", result.RefreshToken);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://accounts.spotify.com/api/token", handler.LastRequest?.RequestUri?.ToString());
    }
}