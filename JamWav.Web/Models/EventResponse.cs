namespace JamWav.Web.Models
{
    public class EventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = string.Empty;
        public Guid BandId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}