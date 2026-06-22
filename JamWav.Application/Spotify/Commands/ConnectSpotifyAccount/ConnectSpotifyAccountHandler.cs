using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using MediatR;

namespace JamWav.Application.Spotify.Commands.ConnectSpotifyAccount;

public class ConnectSpotifyAccountHandler : IRequestHandler<ConnectSpotifyAccountCommand, UserMusicProfile>
{
    private readonly ISpotifyService _spotifyService;
    private readonly IUserMusicProfileRepository _userMusicProfileRepository;

    public ConnectSpotifyAccountHandler(ISpotifyService spotifyService, IUserMusicProfileRepository userMusicProfileRepository)
    {
        _spotifyService = spotifyService;
        _userMusicProfileRepository = userMusicProfileRepository;
    }
    
    public async Task<UserMusicProfile> Handle(ConnectSpotifyAccountCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _spotifyService.ExchangeCodeForTokenAsync(request.Code, cancellationToken);
        var profile = await _spotifyService.GetCurrentUserProfileAsync(tokenResult.AccessToken, cancellationToken);
        var musicTaste = await _spotifyService.GetTopMusicAsync(tokenResult.AccessToken, cancellationToken);
        var existing = await _userMusicProfileRepository.GetByUserIdAsync(request.UserId);

        if (existing == null)
        {
            var newProfile = new UserMusicProfile(
                request.UserId,
                musicTaste.TopArtists,
                musicTaste.TopTracks,
                musicTaste.TopGenres,
                profile.SpotifyUserId,
                tokenResult.AccessToken,
                tokenResult.RefreshToken ?? string.Empty,
                tokenResult.ExpiresAt,
                DateTime.UtcNow);
            await _userMusicProfileRepository.AddAsync(newProfile);
            return newProfile;
        }
        
        existing.UpdateTokens(tokenResult.AccessToken, tokenResult.RefreshToken ?? string.Empty, tokenResult.ExpiresAt);
        existing.UpdateMusicData(musicTaste.TopArtists, musicTaste.TopTracks, musicTaste.TopGenres);
        await _userMusicProfileRepository.UpdateAsync(existing);
        return existing;
    }
}