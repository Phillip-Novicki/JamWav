namespace JamWav.Application.Spotify;

public record SpotifyTokenResult(string AccessToken, string? RefreshToken, DateTime ExpiresAt);

public record SpotifyProfile(string SpotifyUserId, string? DisplayName);

public record SpotifyMusicTaste(List<string> TopArtists, List<string> TopTracks, List<string> TopGenres);
