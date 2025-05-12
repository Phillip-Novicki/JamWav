using System;
using Xunit;
using JamWav.Domain.Entities;

namespace JamWav.Domain.Tests.Entities
{
    public class EventTests
    {
        [Fact]
        public void Ctor_WithFutureDate_InitializesProperties()
        {
            // Arrange
            var title = "Future Event";
            var date  = DateTime.UtcNow.AddDays(1);
            var venue = "Test Venue";

            // Act
            var ev = new Event(title, date, venue);

            // Assert
            Assert.Equal(title, ev.Title);
            Assert.Equal(date,  ev.Date);
            Assert.Equal(venue, ev.Venue);
            Assert.True(ev.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Ctor_WithPastDate_ThrowsArgumentException()
        {
            // Arrange
            var title = "Past Event";
            var date  = DateTime.UtcNow.AddDays(-1);
            var venue = "Test Venue";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Event(title, date, venue));
        }

        [Fact]
        public void Ctor_WithBandId_CreatesCorrectly()
        {
            // Arrange
            var title  = "Band Event";
            var date   = DateTime.UtcNow.AddDays(2);
            var venue  = "Concert Hall";
            var bandId = Guid.NewGuid();

            // Act
            var ev = new Event(title, date, venue, bandId);

            // Assert
            Assert.Equal(title,  ev.Title);
            Assert.Equal(date,   ev.Date);
            Assert.Equal(venue,  ev.Venue);
            Assert.Equal(bandId, ev.BandId);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesProperties()
        {
            // Arrange
            var ev = new Event("Initial Event", DateTime.UtcNow.AddDays(1), "Initial Venue");
            var newTitle  = "Updated Event";
            var newDate   = DateTime.UtcNow.AddDays(2);
            var newVenue  = "New Venue";

            // Act
            ev.UpdateDetails(newTitle, newDate, newVenue);

            // Assert
            Assert.Equal(newTitle, ev.Title);
            Assert.Equal(newDate,  ev.Date);
            Assert.Equal(newVenue, ev.Venue);
        }

        [Fact]
        public void UpdateDetails_WithPastDate_ThrowsArgumentException()
        {
            // Arrange
            var ev = new Event("Initial", DateTime.UtcNow.AddDays(1), "Venue");
            var past = DateTime.UtcNow.AddDays(-2);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ev.UpdateDetails("Name", past, "Location"));
        }
    }
}
