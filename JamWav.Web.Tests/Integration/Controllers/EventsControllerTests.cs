using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JamWav.Web.Models;
using JamWav.Web.Tests.Utils;
using Xunit;

namespace JamWav.Web.Tests.Integration.Controllers
{
    public class EventsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EventsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostEvent_ShouldCreateEvent()
        {
            // Arrange
            var request = new CreateEventRequest
            {
                Title      = "Test Event",
                Date = new DateTime(2025, 5, 15),
                Venue  = "Test Venue"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/events", request);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<EventResponse>();

            // Assert
            Assert.NotNull(created);
            Assert.Equal(request.Title,      created!.Title);
            Assert.Equal(request.Date, created.Date);
            Assert.Equal(request.Venue,  created.Venue);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnEventList()
        {
            // Arrange
            var req = new CreateEventRequest
            {
                Title      = "List Event",
                Date = new DateTime(2025, 6, 1),
                Venue  = "Main Hall"
            };
            await _client.PostAsJsonAsync("/api/events", req);

            // Act
            var response = await _client.GetAsync("/api/events");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<EventResponse>>();

            // Assert
            Assert.Contains(list!, e => 
                e.Title == req.Title 
                && e.Date == req.Date 
                && e.Venue == req.Venue);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnSingleEvent()
        {
            // Arrange & create
            var req = new CreateEventRequest
            {
                Title      = "GetById Event",
                Date = new DateTime(2025, 7, 20),
                Venue  = "Open Air"
            };
            var post = await _client.PostAsJsonAsync("/api/events", req);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<EventResponse>();

            // Act
            var get = await _client.GetAsync($"/api/events/{created!.Id}");
            get.EnsureSuccessStatusCode();
            var fetched = await get.Content.ReadFromJsonAsync<EventResponse>();

            // Assert
            Assert.Equal(created.Id,        fetched!.Id);
            Assert.Equal(req.Title,          fetched.Title);
            Assert.Equal(req.Date,     fetched.Date);
            Assert.Equal(req.Venue,      fetched.Venue);
        }
    }
}
