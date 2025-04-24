using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JamWav.Web.Models;
using JamWav.Web.Tests.Utils;
using Xunit;


namespace JamWav.Web.Tests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostUser_ShouldCreateUser()
    {
        var request = new CreateUserRequest
        {
            Username = "apitestuser",
            Email = "api@example.com",
            DisplayName = "API Test"
        };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);
        Assert.Equal("apitestuser", user!.Username);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUserList()
    {
        await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            Username = "listuser",
            Email = "list@example.com",
            DisplayName = "List User"
        });

        var response = await _client.GetAsync("/api/users");
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        Assert.Contains(users!, u => u.Username == "listuser");
    }

    [Fact]
    public async Task GetUserById_ShouldReturnSingleUser()
    {
        var create = new CreateUserRequest
        {
            Username = "getbyiduser",
            Email = "getbyid@example.com",
            DisplayName = "GetById"
        };

        var post = await _client.PostAsJsonAsync("/api/users", create);
        var created = await post.Content.ReadFromJsonAsync<UserResponse>();

        var get = await _client.GetAsync($"/api/users/{created!.Id}");
        get.EnsureSuccessStatusCode();

        var user = await get.Content.ReadFromJsonAsync<UserResponse>();
        Assert.Equal("getbyiduser", user!.Username);
    }
}
