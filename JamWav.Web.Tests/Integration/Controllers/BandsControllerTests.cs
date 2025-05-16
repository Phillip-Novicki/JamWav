using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JamWav.Web.Models;
using JamWav.Web.Tests.Integration.Utils;
using Xunit;
using Xunit.Abstractions;

namespace JamWav.Web.Tests.Integration.Controllers
{
    public class BandsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient          _client;
        private readonly ITestOutputHelper   _output;

        public BandsControllerTests(CustomWebApplicationFactory<Program> factory,
                                    ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task CreateBand_ShouldCreateBand()
        {
            // Arrange
            var request = new CreateBandRequest { Name = "Test Band" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bands", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<BandResponse>();
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Name);
            Assert.NotEqual(Guid.Empty, created.Id);
        }

        [Fact]
        public async Task GetAllBands_ShouldReturnListIncludingCreated()
        {
            // Arrange: create a band
            var request = new CreateBandRequest { Name = "List Band" };
            var post    = await _client.PostAsJsonAsync("/api/bands", request);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<BandResponse>();

            // Act
            var response = await _client.GetAsync("/api/bands");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var list = await response.Content.ReadFromJsonAsync<List<BandResponse>>();
            Assert.Contains(list!, b => b.Id == created!.Id && b.Name == request.Name);
        }

        [Fact]
        public async Task GetBandById_ShouldReturnSingleBand()
        {
            // Arrange: create a band
            var request = new CreateBandRequest { Name = "Single Band" };
            var post    = await _client.PostAsJsonAsync("/api/bands", request);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<BandResponse>();

            // Act
            var response = await _client.GetAsync($"/api/bands/{created!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var band = await response.Content.ReadFromJsonAsync<BandResponse>();
            Assert.Equal(created.Id, band!.Id);
            Assert.Equal(request.Name, band.Name);
        }

        [Fact]
        public async Task GetBandById_InvalidId_ShouldReturnNotFound()
        {
            // Act
            var invalidId = Guid.NewGuid();
            var response  = await _client.GetAsync($"/api/bands/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateBand_DuplicateName_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateBandRequest { Name = "Dup Band" };
            var first   = await _client.PostAsJsonAsync("/api/bands", request);
            first.EnsureSuccessStatusCode();

            // Act
            var second = await _client.PostAsJsonAsync("/api/bands", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
            var body = await second.Content.ReadAsStringAsync();
            Assert.Contains($"Band `{request.Name}` already exists", body);
        }
    }
}