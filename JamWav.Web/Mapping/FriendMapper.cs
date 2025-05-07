using JamWav.Domain.Entities;
using JamWav.Web.Models;

namespace JamWav.Web.Mapping
{
    public static class FriendMapper
    {
        public static FriendResponse ToResponse(this Friend friend)
        {
            return new FriendResponse
            {
                Id            = friend.Id,
                UserId        = friend.UserId,        
                FriendUserId  = friend.FriendUserId,  
                FriendName    = friend.FriendName,
                CreatedAt     = friend.CreatedAt,
            };
        }

        public static Friend ToEntity(this CreateFriendRequest request)
        {
            return new Friend(
                request.UserId,
                request.FriendUserId,
                request.FriendName);
        }
    }
}
