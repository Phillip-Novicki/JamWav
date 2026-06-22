using JamWav.Application.Interfaces;
using JamWav.Application.Spotify;
using JamWav.Application.Spotify.Commands.ConnectSpotifyAccount;
using JamWav.Domain.Entities;
using Moq;
using Xunit;

namespace JamWav.Application.Tests.Spotify.Commands.ConnectSpotifyAccount;

public class ConnectSpotifyAccountHandlerTests
{
    private readonly Mock<ISpotifyService> _spotifyServiceMock = new();
    private readonly Mock<IUserMusicProfileRepository> _repositoryMock = new();
    private readonly ConnectSpotifyAccountHandler _sut;

    public ConnectSpotifyAccountHandlerTests()
    {
        _sut = new ConnectSpotifyAccountHandler(_spotifyServiceMock.Object, _repositoryMock.Object);
    }

    private void SetUpSpotifyServiceResponses()
    {
        _spotifyServiceMock.Setup(s => s.ExchangeCodeForTokenAsync("test-code", default))
            .ReturnsAsync(new SpotifyTokenResult("test-access-token", "test-refresh-token", DateTime.UtcNow.AddHours(1)));

        _spotifyServiceMock.Setup(s => s.GetCurrentUserProfileAsync("test-access-token", default))
            .ReturnsAsync(new SpotifyProfile("test-spotify-user-id", "Test User"));

        _spotifyServiceMock.Setup(s => s.GetTopMusicAsync("test-access-token", default))
            .ReturnsAsync(new SpotifyMusicTaste(
                new List<string> { "Test Artist" },
                new List<string> { "Test Track" },
                new List<string> { "Test Genre" }));
    }

    [Fact]
    public async Task Handle_NoExistingProfile_CreatesNewUserMusicProfile()
    {
        // Arrange
        SetUpSpotifyServiceResponses();
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((UserMusicProfile?)null);

        UserMusicProfile? added = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserMusicProfile>()))
            .Callback<UserMusicProfile>(p => added = p)
            .Returns(Task.CompletedTask);

        var command = new ConnectSpotifyAccountCommand(userId, "test-code");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserMusicProfile>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserMusicProfile>()), Times.Never);
        Assert.NotNull(added);
        Assert.Equal(userId, added!.UserId);
        Assert.Equal("test-spotify-user-id", added.SpotifyUserId);
        Assert.Equal("test-access-token", added.SpotifyAccessToken);
        Assert.Equal(new[] { "Test Artist" }, added.TopArtists);
        Assert.Same(added, result);
    }

    [Fact]
    public async Task Handle_ExistingProfile_UpdatesTokensAndMusicData()
    {
        // Arrange
        SetUpSpotifyServiceResponses();
        var userId = Guid.NewGuid();
        var existing = new UserMusicProfile(
            userId,
            new List<string> { "Old Artist" },
            new List<string> { "Old Track" },
            new List<string> { "Old Genre" },
            "test-spotify-user-id",
            "old-access-token",
            "old-refresh-token",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddDays(-1));

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var command = new ConnectSpotifyAccountCommand(userId, "test-code");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserMusicProfile>()), Times.Never);
        Assert.Equal("test-access-token", existing.SpotifyAccessToken);
        Assert.Equal(new[] { "Test Artist" }, existing.TopArtists);
        Assert.Same(existing, result);
    }
}
