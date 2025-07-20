using FluentAssertions;
using GAC.Integration.API.Controllers;
using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GAC.Integration.Tests.API.Controllers
{
    public class SalesOrdersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SalesOrdersController _controller;

        public SalesOrdersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new SalesOrdersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithSalesOrders()
        {
            var orders = new List<SalesOrder>
            {
                new SalesOrder
                {
                    Id = Guid.NewGuid(),
                    ProcessingDate = DateTime.UtcNow,
                    ShipmentAddress = "Test Address",
                    CustomerId = Guid.NewGuid(),
                    Lines = new List<SalesOrderItem>()
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllSalesOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<SalesOrder>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResultWithId()
        {
            var command = new CreateSalesOrderCommand(
                DateTime.UtcNow,
                "Test Address",
                Guid.NewGuid(),
                new List<SalesOrderLineDto>
                {
                    new SalesOrderLineDto(Guid.NewGuid(), 2)
                });

            var expectedId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var result = await _controller.Create(command);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetAll), createdAtActionResult.ActionName);
            Assert.Equal(expectedId, createdAtActionResult.Value);
        }
        [Fact]
        public async Task GetById_WhenSalesOrderExists_ShouldReturnOkWithSalesOrder()
        {
            var salesOrderId = Guid.NewGuid();
            var salesOrder = new SalesOrder
            {
                Id = salesOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                ShipmentAddress = "123 Test St",
                Lines = new List<SalesOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetSalesOrderByIdQuery>(q => q.Id == salesOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(salesOrder);

            var result = await _controller.GetById(salesOrderId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(salesOrder);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetSalesOrderByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenSalesOrderDoesNotExist_ShouldReturnNotFound()
        {
            var salesOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetSalesOrderByIdQuery>(q => q.Id == salesOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SalesOrder?)null);

            var result = await _controller.GetById(salesOrderId);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetSalesOrderByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenSalesOrderExists_ShouldReturnNoContent()
        {
            var salesOrderId = Guid.NewGuid();
            var command = new UpdateSalesOrderCommand
            {
                Id = salesOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                ShipmentAddress = "789 Valid St",
                Lines = new List<SalesOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.Update(salesOrderId, command);

            result.Should().BeOfType<NoContentResult>();

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenSalesOrderDoesNotExist_ShouldReturnNotFound()
        {
            var salesOrderId = Guid.NewGuid();
            var command = new UpdateSalesOrderCommand
            {
                Id = salesOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                ShipmentAddress = "000 Not Found St",
                Lines = new List<SalesOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Update(salesOrderId, command);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSalesOrderExists_ShouldReturnNoContent()
        {
            var salesOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteSalesOrderCommand>(c => c.Id == salesOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.Delete(salesOrderId);

            result.Should().BeOfType<NoContentResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteSalesOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSalesOrderDoesNotExist_ShouldReturnNotFound()
        {
            var salesOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteSalesOrderCommand>(c => c.Id == salesOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Delete(salesOrderId);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteSalesOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
