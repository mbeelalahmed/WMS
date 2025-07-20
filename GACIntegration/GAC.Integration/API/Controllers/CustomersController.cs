using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using GAC.Integration.Domain.Interfaces;
using GAC.Integration.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAC.Integration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
            return customer is null ? NotFound() : Ok(customer);
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
        {
            if (id != command.Id) return BadRequest("Mismatched ID");
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteCustomerCommand(id));
            return success ? NoContent() : NotFound();
        }
    }
}
