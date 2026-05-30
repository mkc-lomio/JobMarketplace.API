using JobMarketplace.Application.Features.Skills.Commands.CreateSkill;
using JobMarketplace.Application.Features.Skills.Commands.UpdateSkill;
using JobMarketplace.Application.Features.Skills.Commands.DeleteSkill;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SkillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        //  [Authorize(Roles = "JobSeeker,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateSkillCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPut("{publicGuid:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid publicGuid, [FromBody] UpdateSkillCommand command)
        {
            if (publicGuid != command.PublicGuid)
                return BadRequest(new { error = "Route PublicGuid and body PublicGuid mismatch." });

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }

        [HttpDelete("{publicGuid:guid}")]
        //[Authorize(Roles = "Employer,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid publicGuid)
        {
            var result = await _mediator.Send(new DeleteSkillCommand(publicGuid));
            return result.IsSuccess ? NoContent() : NotFound(result);
        }
    }
}
