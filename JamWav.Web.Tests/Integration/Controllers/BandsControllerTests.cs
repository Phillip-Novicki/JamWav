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
        private readonly HttpClient        _client;
        private readonly ITestOutputHelper _output;

        public BandsControllerTests(CustomWebApplicationFactory<Program> factory,
                                    ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task CreateBand_ShouldCreateBand()
        {
            var request = new CreateBandRequest { Name = "Test Band" };
            var response = await _client.PostAsJsonAsync("/api/bands", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<BandResponse>();
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Name);
            Assert.NotEqual(Guid.Empty, created.Id);
        }

        [Fact]
        public async Task GetAllBands_ShouldReturnListIncludingCreated()
        {
            var request = new CreateBandRequest { Name = "List Band" };
            var post    = await _client.PostAsJsonAsync("/api/bands", request);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<BandResponse>();

            var response = await _client.GetAsync("/api/bands");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var list = await response.Content.ReadFromJsonAsync<List<BandResponse>>();
            Assert.Contains(list!, b => b.Id == created!.Id && b.Name == request.Name);
        }

        [Fact]
        public async Task GetBandById_ShouldReturnSingleBand()
        {
            var request = new CreateBandRequest { Name = "Single Band" };
            var post    = await _client.PostAsJsonAsync("/api/bands", request);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<BandResponse>();

            var response = await _client.GetAsync($"/api/bands/{created!.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var band = await response.Content.ReadFromJsonAsync<BandResponse>();
            Assert.Equal(created.Id, band!.Id);
            Assert.Equal(request.Name, band.Name);
        }

        [Fact]
        public async Task GetBandById_InvalidId_ShouldReturnNotFound()
        {
            var invalidId = Guid.NewGuid();
            var response  = await _client.GetAsync($"/api/bands/{invalidId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateBand_DuplicateName_ShouldReturnBadRequest()
        {
            var request = new CreateBandRequest { Name = "Dup Band" };
            var first   = await _client.PostAsJsonAsync("/api/bands", request);
            first.EnsureSuccessStatusCode();

            var second = await _client.PostAsJsonAsync("/api/bands", request);
            Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
            var body = await second.Content.ReadAsStringAsync();
            Assert.Contains($"Band `{request.Name}` already exists", body);
        }

        [Fact]
        public async Task UpdateBand_ShouldReturnNoContentAndUpdateName()
        {
            // Arrange: create a band
            var createReq = new CreateBandRequest { Name = "Initial Band" };
            var createResp = await _client.PostAsJsonAsync("/api/bands", createReq);
            createResp.EnsureSuccessStatusCode();
            var created = await createResp.Content.ReadFromJsonAsync<BandResponse>();

            // Act: update
            var updateReq = new UpdateBandRequest { Name = "Updated Band" };
            var updateResp = await _client.PutAsJsonAsync($"/api/bands/{created!.Id}", updateReq);

            // Assert: no content
            Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);

            // Verify change
            var getResp = await _client.GetAsync($"/api/bands/{created.Id}");
            getResp.EnsureSuccessStatusCode();
            var updated = await getResp.Content.ReadFromJsonAsync<BandResponse>();
            Assert.Equal(updateReq.Name, updated!.Name);
        }

        [Fact]
        public async Task DeleteBand_ShouldReturnNoContentAndThenNotFound()
        {
            // Arrange: create a band
            var req = new CreateBandRequest { Name = "ToDelete Band" };
            var post = await _client.PostAsJsonAsync("/api/bands", req);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<BandResponse>();

            // Act: delete
            var deleteResp = await _client.DeleteAsync($"/api/bands/{created!.Id}");

            // Assert: no content
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

            // Verify gone
            var getResp = await _client.GetAsync($"/api/bands/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
        }
    }
}
