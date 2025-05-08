using JamWav.Domain.Entities;
using JamWav.Web.Models;

namespace JamWav.Web.Mapping
{
    public static class EventMapper
    {
        public static EventResponse ToResponse(this Event e) => new EventResponse
        {
            Id        = e.Id,
            Title    = e.Title,
            Date      = e.Date,
            Venue    = e.Venue,
            BandId = e.BandId,
            CreatedAt = e.CreatedAt
        };

        public static Event ToEntity(this CreateEventRequest r) => new Event(
            r.Title,
            r.Date,
            r.Venue,
            r.BandId
        );
    }
}