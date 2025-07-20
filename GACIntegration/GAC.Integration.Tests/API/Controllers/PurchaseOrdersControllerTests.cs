using FluentAssertions;
using GAC.Integration.API.Controllers;
using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.Tests.API.Controllers
{
    public class PurchaseOrdersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PurchaseOrdersController _controller;

        public PurchaseOrdersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PurchaseOrdersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfPurchaseOrders()
        {
            var expectedOrders = new List<PurchaseOrder>
            {
                new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    ProcessingDate = DateTime.UtcNow,
                    Lines = new List<PurchaseOrderItem>
                    {
                        new PurchaseOrderItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2 }
                    }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllPurchaseOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedOrders);

            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualOrders = Assert.IsAssignableFrom<List<PurchaseOrder>>(okResult.Value);
            Assert.Single(actualOrders);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            var command = new CreatePurchaseOrderCommand
            (
                ProcessingDate: DateTime.UtcNow,
                CustomerId: Guid.NewGuid(),
                Lines: new List<PurchaseOrderLineDto>
                {
                    new(Guid.NewGuid(), 5)
                }
            );

            var createdId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdId);

            var result = await _controller.Create(command);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetAll), createdResult.ActionName);
            Assert.Equal(createdId, createdResult.Value);
        }

        [Fact]
        public async Task GetById_WhenPurchaseOrderExists_ShouldReturnOkWithPurchaseOrder()
        {
            var purchaseOrderId = Guid.NewGuid();
            var purchaseOrder = new PurchaseOrder
            {
                Id = purchaseOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetPurchaseOrderByIdQuery>(q => q.Id == purchaseOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchaseOrder);

            var result = await _controller.GetById(purchaseOrderId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(purchaseOrder);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetPurchaseOrderByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenPurchaseOrderDoesNotExist_ShouldReturnNotFound()
        {
            var purchaseOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetPurchaseOrderByIdQuery>(q => q.Id == purchaseOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PurchaseOrder?)null);

            var result = await _controller.GetById(purchaseOrderId);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetPurchaseOrderByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
        {
            var command = new UpdatePurchaseOrderCommand
            {
                Id = Guid.NewGuid(),
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>()
            };

            var differentId = Guid.NewGuid();

            var result = await _controller.Update(differentId, command);

            var badRequestResult = result.Should().BeOfType<BadRequestResult>().Subject;

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdatePurchaseOrderCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenPurchaseOrderExists_ShouldReturnNoContent()
        {
            var purchaseOrderId = Guid.NewGuid();
            var command = new UpdatePurchaseOrderCommand
            {
                Id = purchaseOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.Update(purchaseOrderId, command);

            result.Should().BeOfType<NoContentResult>();

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenPurchaseOrderDoesNotExist_ShouldReturnNotFound()
        {
            var purchaseOrderId = Guid.NewGuid();
            var command = new UpdatePurchaseOrderCommand
            {
                Id = purchaseOrderId,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>()
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Update(purchaseOrderId, command);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenPurchaseOrderExists_ShouldReturnNoContent()
        {
            var purchaseOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeletePurchaseOrderCommand>(c => c.Id == purchaseOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.Delete(purchaseOrderId);

            result.Should().BeOfType<NoContentResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeletePurchaseOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenPurchaseOrderDoesNotExist_ShouldReturnNotFound()
        {
            var purchaseOrderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeletePurchaseOrderCommand>(c => c.Id == purchaseOrderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Delete(purchaseOrderId);

            result.Should().BeOfType<NotFoundResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeletePurchaseOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
