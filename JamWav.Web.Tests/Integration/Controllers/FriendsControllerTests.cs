using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JamWav.Web.Models;
using JamWav.Web.Tests.Integration.Utils;
using Xunit;

namespace JamWav.Web.Tests.Integration.Controllers
{
    public class FriendsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public FriendsControllerTests(CustomWebApplicationFactory<Program> factory)
            => _client = factory.CreateClient();

        [Fact]
        public async Task PostFriend_CreatesAndReturnsFriend()
        {
            // 1) Create two users via the API so they exist in the test DB
            var u1 = new CreateUserRequest {
                Username = "alice",
                Email    = "alice@example.com",
                DisplayName = "Alice"
            };
            var res1 = await _client.PostAsJsonAsync("/api/users", u1);
            res1.EnsureSuccessStatusCode();
            var alice = await res1.Content.ReadFromJsonAsync<UserResponse>();

            var u2 = new CreateUserRequest {
                Username = "bob",
                Email    = "bob@example.com",
                DisplayName = "Bob"
            };
            var res2 = await _client.PostAsJsonAsync("/api/users", u2);
            res2.EnsureSuccessStatusCode();
            var bob = await res2.Content.ReadFromJsonAsync<UserResponse>();

            // 2) Now create the friendship between those two real users
            var friendReq = new CreateFriendRequest {
                UserId       = alice!.Id,
                FriendUserId = bob!.Id,
                FriendName   = "Best Buddy"
            };
    
            var post = await _client.PostAsJsonAsync("/api/friends", friendReq);
            post.EnsureSuccessStatusCode();

            var friend = await post.Content.ReadFromJsonAsync<FriendResponse>();
            Assert.NotNull(friend);
            Assert.Equal("Best Buddy", friend!.FriendName);
            Assert.Equal(alice.Id, friend.UserId);

            // 3) Fetch it back by its new ID
            var get = await _client.GetAsync($"/api/friends/{friend.Id}");
            get.EnsureSuccessStatusCode();
            var fetched = await get.Content.ReadFromJsonAsync<FriendResponse>();
            Assert.Equal(friend.UserId, fetched!.UserId);
        }

    }
}