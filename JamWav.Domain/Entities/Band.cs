namespace JamWav.Domain.Entities;

public class Band : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Origin { get; set;} = string.Empty;
    
    // EF Core parameterless constructor
    private Band() {}

    public Band(string name, string genre, string origin)
    {
        Name = name;
        Genre = genre;
        Origin = origin;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdateDetails(string name, string genre, string origin)
    {
        Name = name;
        Genre = genre;
        Origin = origin;
    }
}

