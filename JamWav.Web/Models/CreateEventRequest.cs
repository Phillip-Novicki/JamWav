namespace JamWav.Web.Models
{
    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = string.Empty;
        public Guid BandId { get; set; }
    }
}

