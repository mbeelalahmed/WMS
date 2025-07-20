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
    public class ProductsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WithProductId()
        {
            var expectedId = Guid.NewGuid();
            var command = new CreateProductCommand("P001", "Product 1", "Test product", "10x20");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var result = await _controller.Create(command);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedId);

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithListOfProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Code = "P001", Title = "Product A", Description = "Desc A", Dimensions = "5x5" },
                new Product { Id = Guid.NewGuid(), Code = "P002", Title = "Product B", Description = "Desc B", Dimensions = "10x10" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var result = await _controller.GetAll();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(products);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
