using JobMarketplace.Application.Features.Companies.Commands.CreateCompany;
using JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies;
using JobMarketplace.Application.Features.Companies.Queries.GetCompanyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { publicGuid = result.Data }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// GET /api/companies?pageSize=20&cursor=0
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize = 20, [FromQuery] long cursor = 0)
        {
            var result = await _mediator.Send(new GetAllCompaniesQuery(pageSize, cursor));
            return Ok(result);
        }

        [HttpGet("{publicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetById(Guid publicGuid)
        {
            var result = await _mediator.Send(new GetCompanyByIdQuery(publicGuid));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result);
        }
    }
}