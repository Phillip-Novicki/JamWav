namespace JamWav.Domain.Entities;

public class UserMusicProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    
    public List<string> TopArtists { get; private set; } = new List<string>();

    public List<string> TopTracks { get; private set; } = new List<string>();
    
    public List<string> TopGenres { get; private set; } = new List<string>();
    
    public string SpotifyUserId { get; private set; } = string.Empty;
    
    public string SpotifyAccessToken { get; private set; } = string.Empty;
    
    public string SpotifyRefreshToken { get; private set; } = string.Empty;
    
    public DateTime TokenExpiresAt { get; private set; } = DateTime.MinValue;
    
    public DateTime LastSyncedAt { get; private set; } = DateTime.MinValue;
    
    // EF Core Parameterless constructor
    private UserMusicProfile() { }

    public UserMusicProfile(Guid userId, List<string> topArtists, List<string> topTracks, List<string> topGenres,
        string spotifyUserId, string spotifyAccessToken, string spotifyRefreshToken, DateTime tokenExpiresAt,
        DateTime lastSyncedAt)
    {
        UserId = userId;
        TopArtists = topArtists;
        TopTracks = topTracks;
        TopGenres = topGenres;
        SpotifyUserId = spotifyUserId;
        SpotifyAccessToken = spotifyAccessToken;
        SpotifyRefreshToken = spotifyRefreshToken;
        TokenExpiresAt = tokenExpiresAt;
        LastSyncedAt = lastSyncedAt;
    }

    public void UpdateTokens(string accessToken, string refreshToken, DateTime tokenExpiresAt)
    {
        SpotifyAccessToken = accessToken;
        SpotifyRefreshToken = refreshToken;
        TokenExpiresAt = tokenExpiresAt;
    }

    public void UpdateMusicData(List<string> topArtists, List<string> topTracks, List<string> topGenres)
    {
        TopArtists = topArtists;
        TopTracks = topTracks;
        TopGenres = topGenres;
    }
}
