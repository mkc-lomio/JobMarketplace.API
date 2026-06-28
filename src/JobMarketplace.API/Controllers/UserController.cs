using JobMarketplace.Application.Common.ViewModels;
using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]

        public async Task<IActionResult> Create([FromBody] CreateSkillViewModel command)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize = 20, [FromQuery] long cursor = 0)
        {
            return Ok();
        }
    }
}
