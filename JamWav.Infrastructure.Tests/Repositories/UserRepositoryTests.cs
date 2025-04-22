using System;
using System.Threading.Tasks;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Repositories;
using JamWav.Infrastructure.Tests.Utils;
using Xunit;

namespace JamWav.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);

        var user = new User("testuser", "test@example.com", "Test User");
            
        // Act
        await repo.AddAsync(user);
        var result = await repo.GetByIdAsync(user.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result?.Username);
        Assert.Equal("test@example.com", result?.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        
        // Act
        var result = await repo.GetByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
        
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        
        // Act
        var user = new User("uniqueuser", "unique@example.com", "Unique User");
        await repo.AddAsync(user);
        
        var exists = await repo.UsernameExistsAsync("uniqueuser");
        
        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        
        // Act
        var exists = await repo.UsernameExistsAsync("doesnotexist");
        
        //Assert
        Assert.False(exists);
    }
    
}