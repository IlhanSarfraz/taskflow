using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Projects.Commands.AcceptInvite;
using TaskFlow.Application.Features.Projects.Commands.DeclineInvite;
using TaskFlow.Application.Features.Projects.Queries.GetMyInvites;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class InvitesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvitesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            return Ok(await _mediator.Send(new GetMyInvitesQuery()));
        }

        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> Accept(Guid id)
        {
            await _mediator.Send(new AcceptInviteCommand(id));
            return Ok();
        }

        [HttpPost("{id:guid}/decline")]
        public async Task<IActionResult> Decline(Guid id)
        {
            await _mediator.Send(new DeclineInviteCommand(id));
            return Ok();
        }
    }
}