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
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PurchaseOrdersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<PurchaseOrder>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllPurchaseOrdersQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand order)
        {
            var id = await _mediator.Send(order);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseOrderCommand command)
        {
            if (id != command.Id) return BadRequest();

            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeletePurchaseOrderCommand(id));
            return success ? NoContent() : NotFound();
        }
    }
}
