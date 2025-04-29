using System;

namespace JamWav.Domain.Entities;

public class Friend : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid FriendUserId { get; private set; }

    private Friend() { } // For EF Core

    public Friend(Guid userId, Guid friendUserId)
    {
        UserId = userId;
        FriendUserId = friendUserId;
        CreatedAt = DateTime.UtcNow;
    }
}