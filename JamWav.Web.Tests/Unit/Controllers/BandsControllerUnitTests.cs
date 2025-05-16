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
    public class BandsControllerUnitTests
    {
        private readonly Mock<IBandRepository> _repoMock;
        private readonly BandsController       _controller;

        public BandsControllerUnitTests()
        {
            _repoMock = new Mock<IBandRepository>();
            _controller = new BandsController(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllBands_ReturnsOk_WithListOfBands()
        {
            // Arrange
            var bands = new List<Band>
            {
                new Band("Band1", "Genre1", "Origin1"),
                new Band("Band2", "Genre2", "Origin2")
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bands);

            // Act
            var result = await _controller.GetAllBands();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<BandResponse>>(ok.Value);
            Assert.Collection(returned,
                b => Assert.Equal("Band1", b.Name),
                b => Assert.Equal("Band2", b.Name)
            );
        }

        [Fact]
        public async Task GetBandById_ExistingId_ReturnsOk_WithBand()
        {
            // Arrange
            var band = new Band("BandX", "Punk", "Berlin");
            var id = band.Id;
            _repoMock.Setup(r => r.GetBandById(id)).ReturnsAsync(band);

            // Act
            var result = await _controller.GetBandById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var resp = Assert.IsType<BandResponse>(ok.Value);
            Assert.Equal(id, resp.Id);
            Assert.Equal("BandX", resp.Name);
        }

        [Fact]
        public async Task GetBandById_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetBandById(It.IsAny<Guid>())).ReturnsAsync((Band)null!);

            // Act
            var result = await _controller.GetBandById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateBand_NewName_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new CreateBandRequest { Name = "FreshBand" };
            _repoMock.Setup(r => r.NameExistsAsync(request.Name))
                .ReturnsAsync(false);

            // Simulate EF Core generating an Id on save:
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Band>()))
                .Callback<Band>(b =>
                {
                    // use reflection or a setter if available
                    typeof(BaseEntity)
                        .GetProperty("Id")!
                        .SetValue(b, Guid.NewGuid());
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateBand(request);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var resp    = Assert.IsType<BandResponse>(created.Value!);
            Assert.Equal(request.Name, resp.Name);
            Assert.NotEqual(Guid.Empty, resp.Id); // now it'll pass
        }

        [Fact]
        public async Task CreateBand_DuplicateName_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBandRequest { Name = "Duplicate" };
            _repoMock.Setup(r => r.NameExistsAsync(request.Name)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateBand(request);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("already exists", bad.Value as string);
        }

        [Fact]
        public async Task UpdateBand_Existing_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Band("OldBand", "Jazz", "Paris");
            typeof(BaseEntity).GetProperty("Id")!.SetValue(existing, id);
            _repoMock.Setup(r => r.GetBandById(id)).ReturnsAsync(existing);

            var updateReq = new UpdateBandRequest { Name = "NewBand" };

            // Act
            var result = await _controller.Update(id, updateReq);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("NewBand", existing.Name);
        }

        [Fact]
        public async Task UpdateBand_Nonexistent_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetBandById(id)).ReturnsAsync((Band)null!);
            var updateReq = new UpdateBandRequest { Name = "X" };

            // Act
            var result = await _controller.Update(id, updateReq);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBand_Existing_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBand_Nonexistent_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}