namespace JamWav.Web.Models;

public class CreateFriendRequest
{
    public Guid UserId { get; set; }
    public Guid FriendUserId { get; set; }
    public string FriendName { get; set; } = string.Empty;
}