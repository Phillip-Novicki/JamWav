namespace JamWav.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // EF Core parameterless constructor
    private User() { }

    // Domain constructor
    public User(string username, string email, string displayName)
    {
        Id = Guid.NewGuid();
        SetUsername(username);
        SetEmail(email);
        DisplayName = displayName;
        CreatedAt = DateTime.UtcNow;
    }
    
    private void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        }
        Username = username;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            throw new ArgumentException("Invalid email address.", nameof(email));
        }

        Email = email;
    }

    public void ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        }
        DisplayName = displayName;
    }
}