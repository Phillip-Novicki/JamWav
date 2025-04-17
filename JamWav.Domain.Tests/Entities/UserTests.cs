using System;
using Xunit;
using JamWav.Domain.Entities;

namespace JamWav.Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Ctor_ValidParameters_InitializesProperties()
        {
            // Arrange
            var before = DateTime.UtcNow;
            string username = "testuser";
            string email = "test@example.com";
            string displayName = "Test User";

            // Act
            var user = new User(username, email, displayName);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email);
            Assert.Equal(displayName, user.DisplayName);
            Assert.InRange(user.CreatedAt, before, after);
        }

        [Theory]
        [InlineData("", "test@example.com", "Name")]
        [InlineData(null, "test@example.com", "Name")]
        public void Ctor_InvalidUsername_Throws(string badUsername, string email, string name)
        {
            Assert.Throws<ArgumentException>(() =>
                new User(badUsername, email, name));
        }

        [Theory]
        [InlineData("user", "", "Name")]
        [InlineData("user", "not-an-email", "Name")]
        public void Ctor_InvalidEmail_Throws(string username, string badEmail, string name)
        {
            Assert.Throws<ArgumentException>(() =>
                new User(username, badEmail, name));
        }

        [Fact]
        public void ChangeDisplayName_Valid_UpdatesDisplayName()
        {
            // Arrange
            var user = new User("u", "e@e.com", "Old");
            var newName = "New Name";

            // Act
            user.ChangeDisplayName(newName);

            // Assert
            Assert.Equal(newName, user.DisplayName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ChangeDisplayName_Invalid_Throws(string badName)
        {
            var user = new User("u", "e@e.com", "Old");
            Assert.Throws<ArgumentException>(() =>
                user.ChangeDisplayName(badName));
        }
    }
}
