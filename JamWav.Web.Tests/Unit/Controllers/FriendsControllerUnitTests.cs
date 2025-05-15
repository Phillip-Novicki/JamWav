using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWav.Web.Controllers;
using JamWav.Web.Models;
using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Web.Mapping;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace JamWav.Web.Tests.Unit.Controllers
{
    public class FriendsControllerUnitTests
    {
        [Fact]
        public async Task GetAllFriends_Returns_AllFriends()
        {
            // Arrange
            var f1 = new Friend(Guid.NewGuid(), Guid.NewGuid(), "Alice");
            var f2 = new Friend(Guid.NewGuid(), Guid.NewGuid(), "Bob");
            var list = new List<Friend> { f1, f2 };

            var repo = new Mock<IFriendRepository>();
            repo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(list);

            var ctrl = new FriendsController(repo.Object);

            // Act
            var result = await ctrl.GetAllFriends();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<FriendResponse>>(ok.Value);
            Assert.Contains(returned, x => x.FriendName == "Alice");
            Assert.Contains(returned, x => x.FriendName == "Bob");
        }

        [Fact]
        public async Task GetFriendById_ExistingId_Returns_Friend()
        {
            // Arrange
            var friend = new Friend(Guid.NewGuid(), Guid.NewGuid(), "Charlie");
            var repo = new Mock<IFriendRepository>();
            repo.Setup(r => r.GetByIdAsync(friend.Id))
                .ReturnsAsync(friend);

            var ctrl = new FriendsController(repo.Object);

            // Act
            var result = await ctrl.GetFriendById(friend.Id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<FriendResponse>(ok.Value);
            Assert.Equal("Charlie", returned.FriendName);
        }

        [Fact]
        public async Task GetFriendById_NonexistentId_Returns_NotFound()
        {
            // Arrange
            var repo = new Mock<IFriendRepository>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Friend?)null);

            var ctrl = new FriendsController(repo.Object);

            // Act
            var result = await ctrl.GetFriendById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateFriend_ValidRequest_Returns_Created()
        {
            // Arrange
            var dto = new CreateFriendRequest {
                UserId = Guid.NewGuid(),
                FriendUserId = Guid.NewGuid(),
                FriendName = "David"
            };

            var repo = new Mock<IFriendRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<Friend>()))
                .Returns(Task.CompletedTask);

            var ctrl = new FriendsController(repo.Object);

            // Act
            var result = await ctrl.CreateFriend(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(FriendsController.GetFriendById), created.ActionName);

            var returned = Assert.IsType<FriendResponse>(created.Value);
            Assert.Equal("David", returned.FriendName);
            Assert.Equal(dto.UserId, returned.UserId);
        }
    }
}
