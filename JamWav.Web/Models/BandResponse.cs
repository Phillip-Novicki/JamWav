namespace JamWav.Web.Models;

public class BandResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public string Origin { get; set; }
    public DateTime CreatedAt { get; set; }
}