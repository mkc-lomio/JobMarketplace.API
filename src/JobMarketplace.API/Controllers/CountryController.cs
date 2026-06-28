using JobMarketplace.Application.Features.Applications.Commands.CreateApplication;
using JobMarketplace.Application.Features.Countries.Commands.CreateCountry;
using JobMarketplace.Application.Features.Countries.Commands.DeleteCountry;
using JobMarketplace.Application.Features.Countries.Commands.UpdateCountry;
using JobMarketplace.Application.Features.Jobs.Commands.DeleteJob;
using JobMarketplace.Application.Features.Jobs.Commands.UpdateJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly IMediator _mediator;
    
        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker,Admin")]
        //[AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateCountryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPut("{publicGuid:guid}")]
        //[Authorize(Roles = "Employer,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid publicGuid, [FromBody] UpdateCountryCommand command)
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
            var result = await _mediator.Send(new DeleteCountryCommand(publicGuid));
            return result.IsSuccess ? NoContent() : NotFound(result);
        }
    }
}
