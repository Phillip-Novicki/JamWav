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

namespace JamWav.Web.Tests.Unit.Controllers
{
    public class UsersControllerUnitTests
    {
        [Fact]
        public async Task GetAllUsers_Returns_AllUsers()
        {
            // Arrange
            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "u1",
                Email = "u1@example.com",
                DisplayName = "U1"
            };
            var user2 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "u2",
                Email = "u2@example.com",
                DisplayName = "U2"
            };
            var list = new List<ApplicationUser> { user1, user2 };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(list);

            var ctrl = new UsersController(repo.Object);

            // Act
            var result = await ctrl.GetAllUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(ok.Value);
            Assert.Contains(returned, u => u.Username == "u1");
            Assert.Contains(returned, u => u.Username == "u2");
        }

        [Fact]
        public async Task GetUserById_ExistingId_Returns_User()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = id,
                UserName = "bob",
                Email = "bob@example.com",
                DisplayName = "Bob"
            };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((ApplicationUser?)user);

            var ctrl = new UsersController(repo.Object);

            // Act
            var result = await ctrl.GetUserById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<UserResponse>(ok.Value);
            Assert.Equal("bob", returned.Username);
        }

        [Fact]
        public async Task GetUserById_NonexistentId_Returns_NotFound()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ApplicationUser?)null);

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
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            repo.Setup(r => r.AddAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.CompletedTask);

            var ctrl = new UsersController(repo.Object);
            var dto = new CreateUserRequest
            {
                Username = "alice",
                Email = "alice@example.com",
                DisplayName = "Alice"
            };

            // Act
            var result = await ctrl.CreateUser(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetUserById), created.ActionName);

            var returned = Assert.IsType<UserResponse>(created.Value!);
            Assert.Equal("alice", returned.Username);
        }

        [Fact]
        public async Task CreateUser_DuplicateUsername_Returns_BadRequest()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync("dup"))
                .ReturnsAsync(true);

            var ctrl = new UsersController(repo.Object);
            var dto = new CreateUserRequest
            {
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
