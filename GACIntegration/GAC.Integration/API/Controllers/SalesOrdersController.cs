using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAC.Integration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SalesOrdersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<SalesOrder>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllSalesOrdersQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesOrderCommand order)
        {
            var id = await _mediator.Send(order);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSalesOrderByIdQuery(id));
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalesOrderCommand command)
        {
            if (id != command.Id) return BadRequest();

            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteSalesOrderCommand(id));
            return success ? NoContent() : NotFound();
        }
    }
}
