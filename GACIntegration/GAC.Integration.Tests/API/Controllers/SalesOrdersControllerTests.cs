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
    }
}
