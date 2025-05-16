using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using JamWav.Web.Controllers;
using JamWav.Web.Mapping;
using JamWav.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace JamWav.Web.Tests.Unit.Controllers
{
    public class EventsControllerUnitTests
    {
        private readonly Mock<IEventRepository> _repoMock;
        private readonly EventsController       _controller;

        public EventsControllerUnitTests()
        {
            _repoMock = new Mock<IEventRepository>();
            _controller = new EventsController(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllEvents_WhenCalled_ReturnsOkResultWithEventList()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event("Title1", DateTime.UtcNow.AddDays(1), "Venue1", Guid.NewGuid()),
                new Event("Title2", DateTime.UtcNow.AddDays(2), "Venue2", Guid.NewGuid())
            };
            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(events);

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<EventResponse>>(ok.Value);
            Assert.Collection(list,
                e => Assert.Equal("Title1", e.Title),
                e => Assert.Equal("Title2", e.Title)
            );
        }

        [Fact]
        public async Task GetEventById_ExistingId_ReturnsOkWithEvent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var evt = new Event("TitleX", DateTime.UtcNow.AddDays(3), "VenueX", Guid.NewGuid());
            typeof(BaseEntity).GetProperty("Id")!.SetValue(evt, id);
            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(evt);

            // Act
            var result = await _controller.GetEventById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var resp = Assert.IsType<EventResponse>(ok.Value);
            Assert.Equal(id, resp.Id);
            Assert.Equal("TitleX", resp.Title);
        }

        [Fact]
        public async Task GetEventById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((Event)null!);

            // Act
            var result = await _controller.GetEventById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateEvent_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var req = new CreateEventRequest
            {
                Title = "NewEvent",
                Date  = DateTime.UtcNow.AddDays(5),
                Venue = "TestVenue"
            };
            var entity = req.ToEntity();
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Event>()))
                     .Returns(Task.CompletedTask)
                     .Callback<Event>(e => entity = e);

            // Act
            var result = await _controller.CreateEvent(req);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var resp = Assert.IsType<EventResponse>(created.Value);
            Assert.Equal(req.Title, resp.Title);
            Assert.Equal(req.Date, resp.Date);
        }
    }
}
