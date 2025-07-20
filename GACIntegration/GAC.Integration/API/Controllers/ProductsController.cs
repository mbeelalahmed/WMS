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
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product?>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            return result ? NoContent() : NotFound();
        }
    }
}
