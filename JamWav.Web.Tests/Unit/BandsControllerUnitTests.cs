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

namespace JamWav.Web.Tests.Controllers;

public class BandsControllerUnitTests
{
    [Fact]
    public async Task GetAllBands_Returns_AllBands()
    {
        // Arrange
        var band1 = new Band("Band1", "Rock", "New York");
        var band2 = new Band("Band2", "Metal", "London");
        var list = new List<Band> { band1, band2 };

        var repo = new Mock<IBandRepository>();
        repo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(list);

        var ctrl = new BandsController(repo.Object);

        // Act
        var result = await ctrl.GetAllBands();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<BandResponse>>(ok.Value);
        Assert.Contains(returned, b => b.Name == "Band1");
        Assert.Contains(returned, b => b.Name == "Band2");
    }

    [Fact]
    public async Task GetBandById_ExistingId_Returns_Band()
    {
        // Arrange
        var band = new Band("BandX", "Punk", "Berlin");
        var repo = new Mock<IBandRepository>();
        repo.Setup(r => r.GetBandById(band.Id))
            .ReturnsAsync(band);

        var ctrl = new BandsController(repo.Object);

        // Act
        var result = await ctrl.GetBandById(band.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<BandResponse>(ok.Value);
        Assert.Equal("BandX", returned.Name);
    }

    [Fact]
    public async Task GetBandById_NonexistentId_Returns_NotFound()
    {
        // Arrange
        var repo = new Mock<IBandRepository>();
        repo.Setup(r => r.GetBandById(It.IsAny<Guid>()))
            .ReturnsAsync((Band?)null);

        var ctrl = new BandsController(repo.Object);

        // Act
        var result = await ctrl.GetBandById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateBand_NewName_Returns_Created()
    {
        // Arrange
        var repo = new Mock<IBandRepository>();
        repo.Setup(r => r.NameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var ctrl = new BandsController(repo.Object);

        var dto = new CreateBandRequest
        {
            Name = "Fresh Band",
            Genre = "Pop",
            Origin = "Austin, TX"
        };

        // Act
        var result = await ctrl.CreateBand(dto);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BandsController.GetBandById), created.ActionName);

        var returned = Assert.IsType<BandResponse>(created.Value!);
        Assert.Equal("Fresh Band", returned.Name);
    }

    [Fact]
    public async Task CreateBand_DuplicateName_Returns_BadRequest()
    {
        // Arrange
        var repo = new Mock<IBandRepository>();
        repo.Setup(r => r.NameExistsAsync("DuplicateBand"))
            .ReturnsAsync(true);

        var ctrl = new BandsController(repo.Object);

        var dto = new CreateBandRequest
        {
            Name = "DuplicateBand",
            Genre = "Jazz",
            Origin = "Paris"
        };

        // Act
        var result = await ctrl.CreateBand(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("already exists", bad.Value as string);
    }
}
