using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Application.Features.Jobs.Commands.DeleteJob;
using JobMarketplace.Application.Features.Jobs.Commands.UpdateJob;
using JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs;
using JobMarketplace.Application.Features.Jobs.Queries.GetJobById;
using JobMarketplace.Application.Features.Jobs.Queries.SearchJobs;
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
        private readonly IDapperQueryService _queryService;

        public JobsController(IMediator mediator, IDapperQueryService queryService)
        {
            _mediator = mediator;
            _queryService = queryService;
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

        /// <summary>
        /// GET /api/jobs?pageSize=20&cursor=0
        /// Cursor-based pagination — pass the NextCursor from the previous response.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize = 20, [FromQuery] long cursor = 0)
        {
            var result = await _mediator.Send(new GetAllJobsQuery(pageSize, cursor));
            return Ok(result);
        }

        [HttpGet("{publicGuid:guid}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> GetById(Guid publicGuid)
        {
            var result = await _mediator.Send(new GetJobByIdQuery(publicGuid));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result);
        }

        /// <summary>
        /// GET /api/jobs/search?query=developer&location=Remote&jobType=FullTime&pageSize=20&cursor=0
        /// Full-text search across Title + Description with filters. 3-table join includes application count.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> Search(
            [FromQuery] string? query,
            [FromQuery] string? location,
            [FromQuery] string? jobType,
            [FromQuery] string? experienceLevel,
            [FromQuery] int pageSize = 20,
            [FromQuery] long cursor = 0)
        {
            var result = await _mediator.Send(new SearchJobsQuery
            {
                SearchTerm = query,
                Location = location,
                JobType = jobType,
                ExperienceLevel = experienceLevel,
                PageSize = pageSize,
                Cursor = cursor
            });
            return Ok(result);
        }

        /// <summary>
        /// GET /api/jobs/export
        /// Streams all active jobs as IAsyncEnumerable — starts sending data immediately,
        /// never buffers the full result set in memory. Ideal for large exports.
        /// </summary>
        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public IAsyncEnumerable<JobExportDto> Export(CancellationToken cancellationToken)
        {
            return _queryService.StreamAsync<JobExportDto>(
                "sp_ExportJobs", cancellationToken: cancellationToken);
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