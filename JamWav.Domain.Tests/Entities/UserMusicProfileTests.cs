using JamWav.Domain.Entities;

namespace JamWav.Domain.Tests.Entities;

public class UserMusicProfileTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly List<string> _artists = ["Radiohead", "Bon Iver"];
    private readonly List<string> _tracks = ["Creep", "Skinny Love"];
    private readonly List<string> _genres = ["Alternative", "Indie Folk"];
    private readonly string _spotifyUserId = "spotify:user:phillip123";
    private readonly string _accessToken = "access-token";
    private readonly string _refreshToken = "refresh-token";
    private readonly DateTime _expiresAt = DateTime.UtcNow.AddHours(1);
    private readonly DateTime _lastSynced = DateTime.UtcNow;

    [Fact]
    public void Ctor_SetsAllProperties()
    {
        var profile = new UserMusicProfile(_userId, _artists, _tracks, _genres,
            _spotifyUserId, _accessToken, _refreshToken, _expiresAt, _lastSynced);

        Assert.Equal(_userId, profile.UserId);
        Assert.Equal(_artists, profile.TopArtists);
        Assert.Equal(_tracks, profile.TopTracks);
        Assert.Equal(_genres, profile.TopGenres);
        Assert.Equal(_spotifyUserId, profile.SpotifyUserId);
        Assert.Equal(_accessToken, profile.SpotifyAccessToken);
        Assert.Equal(_refreshToken, profile.SpotifyRefreshToken);
        Assert.Equal(_expiresAt, profile.TokenExpiresAt);
        Assert.Equal(_lastSynced, profile.LastSyncedAt);
    }

    [Fact]
    public void UpdateTokens_UpdatesTokenProperties()
    {
        var profile = new UserMusicProfile(_userId, _artists, _tracks, _genres,
            _spotifyUserId, _accessToken, _refreshToken, _expiresAt, _lastSynced);

        var newAccess = "new-access-token";
        var newRefresh = "new-refresh-token";
        var newExpiry = DateTime.UtcNow.AddHours(2);

        profile.UpdateTokens(newAccess, newRefresh, newExpiry);

        Assert.Equal(newAccess, profile.SpotifyAccessToken);
        Assert.Equal(newRefresh, profile.SpotifyRefreshToken);
        Assert.Equal(newExpiry, profile.TokenExpiresAt);
    }

    [Fact]
    public void UpdateMusicData_UpdatesMusicLists()
    {
        var profile = new UserMusicProfile(_userId, _artists, _tracks, _genres,
            _spotifyUserId, _accessToken, _refreshToken, _expiresAt, _lastSynced);

        var newArtists = new List<string> { "Portishead", "Massive Attack" };
        var newTracks = new List<string> { "Glory Box", "Teardrop" };
        var newGenres = new List<string> { "Trip Hop" };

        profile.UpdateMusicData(newArtists, newTracks, newGenres);

        Assert.Equal(newArtists, profile.TopArtists);
        Assert.Equal(newTracks, profile.TopTracks);
        Assert.Equal(newGenres, profile.TopGenres);
    }
}
