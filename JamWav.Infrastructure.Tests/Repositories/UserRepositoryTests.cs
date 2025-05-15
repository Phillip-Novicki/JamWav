using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence.Repositories;
using JamWav.Infrastructure.Tests.Utils;

namespace JamWav.Infrastructure.Tests.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                DisplayName = "Test User",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await repo.AddAsync(user);
            var result = await repo.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result?.UserName);
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
            await using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "uniqueuser",
                Email = "unique@example.com",
                DisplayName = "Unique User",
                CreatedAt = DateTime.UtcNow
            };
            await repo.AddAsync(user);

            // Act
            var exists = await repo.UsernameExistsAsync("uniqueuser");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UsernameExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);

            // Act
            var exists = await repo.UsernameExistsAsync("doesnotexist");

            // Assert
            Assert.False(exists);
        }
    }
}
