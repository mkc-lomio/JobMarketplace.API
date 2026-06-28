using JobMarketplace.Application.Features.Skills.Commands.CreateSkill;
using JobMarketplace.Application.Features.Skills.Commands.UpdateSkill;
using JobMarketplace.Application.Features.Skills.Commands.DeleteSkill;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobMarketplace.Application.Features.Transactions.Commands;
using JobMarketplace.Application.Features.Transactions.Commands.DeleteTransaction;

namespace JobMarketplace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        //  [Authorize(Roles = "JobSeeker,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }
    }
}
