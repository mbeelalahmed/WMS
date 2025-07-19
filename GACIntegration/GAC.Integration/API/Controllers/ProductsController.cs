﻿using GAC.Integration.Application.Commands;
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
    }
}
