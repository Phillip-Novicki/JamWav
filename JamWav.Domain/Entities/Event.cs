namespace JamWav.Domain.Entities;

public class Event : BaseEntity
{
    public string Title      { get; private set; }
    public DateTime Date     { get; private set; }
    public string Venue      { get; private set; }
    public Guid BandId       { get; private set; }

    private Event() { } 

    public Event(string title, DateTime date, string venue, Guid bandId)
    {
        Title     = title;
        Date      = date;
        Venue     = venue;
        BandId    = bandId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, DateTime date, string venue)
    {
        Title = title;
        Date  = date;
        Venue = venue;
    }
}