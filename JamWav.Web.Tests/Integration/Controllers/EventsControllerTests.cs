using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JamWav.Web.Models;
using JamWav.Web.Tests.Integration.Utils;
using Xunit;
using Xunit.Abstractions;

namespace JamWav.Web.Tests.Integration.Controllers
{
    public class EventsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient         _client;
        private readonly ITestOutputHelper _output;

        public EventsControllerTests(CustomWebApplicationFactory<Program> factory,
                                     ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task PostEvent_ShouldCreateEvent()
        {
            // Arrange: use tomorrow’s date so it’s always in the future
            var tomorrow = DateTime.UtcNow.AddDays(1).Date;
            var request = new CreateEventRequest
            {
                Title = "Test Event",
                Date  = tomorrow,
                Venue = "Test Venue"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/events", request);

            // Dump status & body for debugging
            var body = await response.Content.ReadAsStringAsync();
            _output.WriteLine("POST /api/events → {0}", response.StatusCode);
            _output.WriteLine("Response body:\n{0}", body);

            // Assert
            Assert.True(response.IsSuccessStatusCode,
                $"Expected 2xx but got {(int)response.StatusCode}. See response body above.");

            var created = await response.Content.ReadFromJsonAsync<EventResponse>();
            Assert.NotNull(created);
            Assert.Equal(request.Title, created!.Title);
            Assert.Equal(request.Date,  created.Date);
            Assert.Equal(request.Venue, created.Venue);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnEventList()
        {
            // Arrange: schedule an event far in the future
            var futureDate = DateTime.UtcNow.AddDays(30).Date;
            var req = new CreateEventRequest
            {
                Title = "List Event",
                Date  = futureDate,
                Venue = "Main Hall"
            };
            await _client.PostAsJsonAsync("/api/events", req);

            // Act
            var response = await _client.GetAsync("/api/events");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<EventResponse>>();

            // Assert
            Assert.Contains(list!, e =>
                e.Title == req.Title &&
                e.Date  == req.Date  &&
                e.Venue == req.Venue);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnSingleEvent()
        {
            // Arrange & create
            var futureDate = DateTime.UtcNow.AddDays(30).Date;
            var req = new CreateEventRequest
            {
                Title = "GetById Event",
                Date  = futureDate,
                Venue = "Open Air"
            };
            var post    = await _client.PostAsJsonAsync("/api/events", req);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<EventResponse>();

            // Act
            var get = await _client.GetAsync($"/api/events/{created!.Id}");

            // Dump status + body so we see the real error
            var body = await get.Content.ReadAsStringAsync();
            _output.WriteLine("GET /api/events/{0} → {1}", created.Id, get.StatusCode);
            _output.WriteLine("Response body:\n{0}", body);

            // Assert
            Assert.True(get.IsSuccessStatusCode,
                $"Expected 2xx but got {(int)get.StatusCode} – see logs above for details.");

            var fetched = await get.Content.ReadFromJsonAsync<EventResponse>();
            Assert.Equal(created.Id,    fetched!.Id);
            Assert.Equal(req.Title,     fetched.Title);
            Assert.Equal(req.Date,      fetched.Date);
            Assert.Equal(req.Venue,     fetched.Venue);
        }
    }
}
