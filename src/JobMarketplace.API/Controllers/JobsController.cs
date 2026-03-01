using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Application.Features.Jobs.Commands.DeleteJob;
using JobMarketplace.Application.Features.Jobs.Commands.UpdateJob;
using JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs;
using JobMarketplace.Application.Features.Jobs.Queries.GetJobById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateJobCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { publicGuid = result.Data }, result)
                : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _mediator.Send(new GetAllJobsQuery());
            return Ok(jobs);
        }

        [HttpGet("{publicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetById(Guid publicGuid)
        {
            var result = await _mediator.Send(new GetJobByIdQuery(publicGuid));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result);
        }

        [HttpPut("{publicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Update(Guid publicGuid, [FromBody] UpdateJobCommand command)
        {
            if (publicGuid != command.PublicGuid)
                return BadRequest(new { error = "Route PublicGuid and body PublicGuid mismatch." });

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }

        [HttpDelete("{publicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Delete(Guid publicGuid)
        {
            var result = await _mediator.Send(new DeleteJobCommand(publicGuid));
            return result.IsSuccess ? NoContent() : NotFound(result);
        }
    }
}