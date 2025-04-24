using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWav.Web.Controllers;
using JamWav.Web.Models;
using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace JamWav.Web.Tests.Controllers
{
    public class UsersControllerUnitTests
    {
        [Fact]
        public async Task GetAllUsers_Returns_AllUsers()
        {
            // Arrange
            var user1 = new User("u1", "u1@example.com", "U1");
            var user2 = new User("u2", "u2@example.com", "U2");
            var list = new List<User> { user1, user2 };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult((IEnumerable<User>)list));

            var ctrl = new UsersController(repo.Object);

            // Act
            var result = await ctrl.GetAllUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<User>>(ok.Value);
            Assert.Contains(returned, u => u.Username == "u1");
            Assert.Contains(returned, u => u.Username == "u2");
        }

        [Fact]
        public async Task GetUserById_ExistingId_Returns_User()
        {
            // Arrange
            var user = new User("bob", "bob@example.com", "Bob");
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(user.Id))
                .Returns(Task.FromResult(user));

            var ctrl = new UsersController(repo.Object);

            // Act
            var result = await ctrl.GetUserById(user.Id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<User>(ok.Value);
            Assert.Equal("bob", returned.Username);
        }

        [Fact]
        public async Task GetUserById_NonexistentId_Returns_NotFound()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<User?>(null));

            var ctrl = new UsersController(repo.Object);

            // Act
            var result = await ctrl.GetUserById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateUser_NewUsername_Returns_Created()
        {
            // Arrange
            var createdUser = new User("alice", "alice@example.com", "Alice");
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            repo.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(createdUser));

            var ctrl = new UsersController(repo.Object);
            var dto = new CreateUserRequest {
                Username = "alice",
                Email = "alice@example.com",
                DisplayName = "Alice"
            };

            // Act
            var result = await ctrl.CreateUser(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetAllUsers), created.ActionName);

            var returned = Assert.IsType<User>(created.Value!);
            Assert.Equal("alice", returned.Username);
        }

        [Fact]
        public async Task CreateUser_DuplicateUsername_Returns_BadRequest()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync("dup"))
                .Returns(Task.FromResult(true));

            var ctrl = new UsersController(repo.Object);
            var dto = new CreateUserRequest {
                Username = "dup",
                Email = "dup@example.com",
                DisplayName = "Dup"
            };

            // Act
            var result = await ctrl.CreateUser(dto);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("already exists", bad.Value as string);
        }
    }
}
