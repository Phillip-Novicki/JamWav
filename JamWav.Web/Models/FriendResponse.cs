namespace JamWav.Web.Models;

public class FriendResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendUserId { get; set; }
    public string FriendName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}