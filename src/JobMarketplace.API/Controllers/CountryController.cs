using JobMarketplace.Application.Features.Applications.Commands.CreateApplication;
using JobMarketplace.Application.Features.Countries.Commands.CreateCountry;
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
        //  [Authorize(Roles = "JobSeeker,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateCountryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }
    }
}
