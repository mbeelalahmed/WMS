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
    }
}
