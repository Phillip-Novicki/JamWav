using System;

namespace JamWav.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Title    { get; private set; } = string.Empty;
        public DateTime Date   { get; private set; }
        public string Venue    { get; private set; } = string.Empty;
        public Guid   BandId   { get; private set; }

        private Event() { } // For EF Core

        public Event(string title, DateTime date, string venue, Guid bandId)
        {
            if (date < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past", nameof(date));

            Title     = title;
            Date      = date;
            Venue     = venue;
            BandId    = bandId;
            CreatedAt = DateTime.UtcNow;
        }

        public Event(string title, DateTime date, string venue)
        {
            if (date < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past", nameof(date));

            Title     = title;
            Date      = date;
            Venue     = venue;
            CreatedAt = DateTime.UtcNow;
            // BandId remains default(Guid) if not provided
        }

        public void UpdateDetails(string title, DateTime date, string venue)
        {
            if (date < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past", nameof(date));

            Title = title;
            Date  = date;
            Venue = venue;
        }
    }
}