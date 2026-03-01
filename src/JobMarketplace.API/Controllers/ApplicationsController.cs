using JobMarketplace.Application.Features.Applications.Commands.CreateApplication;
using JobMarketplace.Application.Features.Applications.Queries.GetApplicationsByJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateApplicationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Created("", result) : BadRequest(result);
        }

        [HttpGet("by-job/{jobPublicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetByJob(Guid jobPublicGuid)
        {
            var applications = await _mediator.Send(new GetApplicationsByJobQuery(jobPublicGuid));
            return Ok(applications);
        }
    }
}
