namespace JamWav.Domain.Entities;

public class Friend : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid FriendUserId { get; private set; }
    
    public string FriendName { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Friend() { } // For EF Core

    public Friend(Guid userId, Guid friendUserId, string friendName)
    {
        UserId = userId;
        FriendUserId = friendUserId;
        FriendName = friendName;
        CreatedAt = DateTime.UtcNow;
    }
}