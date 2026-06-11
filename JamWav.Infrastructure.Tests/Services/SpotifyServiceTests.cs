using JamWav.Infrastructure.Services;
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

        var sut = new SpotifyService(config);

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
}