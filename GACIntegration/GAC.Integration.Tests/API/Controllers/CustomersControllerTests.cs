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
    public class CustomersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CustomersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WithCustomerId()
        {
            var expectedId = Guid.NewGuid();
            var command = new CreateCustomerCommand("Test Name", "Dubai");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var result = await _controller.Create(command);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedId);

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithListOfCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), Name = "John", Address = "Dubai" },
                new Customer { Id = Guid.NewGuid(), Name = "Jane", Address = "AbuDhabi" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            var result = await _controller.GetAll();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(customers);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
