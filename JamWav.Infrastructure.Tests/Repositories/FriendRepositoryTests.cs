using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using JamWav.Infrastructure.Persistence.Repositories;
using JamWav.Infrastructure.Tests.Utils;
using JamWav.Domain.Entities;

namespace JamWav.Infrastructure.Tests.Repositories;

public class FriendRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddFriend()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.CreateContext();
        var repo = new FriendRepository(ctx);
        var friend = new Friend(Guid.NewGuid(), Guid.NewGuid(), "Buddy");
        
        // Act
        await repo.AddAsync(friend);
        var loaded = await repo.GetByIdAsync(friend.Id);
        
        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("Buddy", loaded!.FriendName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.CreateContext();
        var repo =  new FriendRepository(ctx);
        
        // Act
        var loaded = await repo.GetByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(loaded);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFriends()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.CreateContext();
        var repo = new FriendRepository(ctx);
        var f1 = new Friend(Guid.NewGuid(), Guid.NewGuid(), "A");
        var f2 = new Friend(Guid.NewGuid(), Guid.NewGuid(), "B");
        await repo.AddAsync(f1);
        await repo.AddAsync(f2);
        
        // Act
        var list = await repo.GetAllAsync();
        
        // Assert
        Assert.Collection(list,
            first => Assert.Equal("A", first!.FriendName),
            second => Assert.Equal("B", second!.FriendName));
    }
}