using System;

namespace JamWav.Domain.Entities;

public class Event : BaseEntity
{
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public string Location { get; private set; }
    
    
    private Event() { } // For EF Core

    public Event(string name, DateTime startDate, string location)
    {
        Name = name;
        StartDate = startDate;
        Location = location;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, DateTime startDate, string location)
    {
        Name = name;
        StartDate = startDate;
        Location = location;
    }
}