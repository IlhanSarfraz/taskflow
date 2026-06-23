using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Notifications.Commands.MarkNotificationRead;
using TaskFlow.Application.Features.Notifications.Queries.GetMyNotifications;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            return Ok(await _mediator.Send(new GetMyNotificationsQuery()));
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            await _mediator.Send(new MarkNotificationReadCommand(id));
            return Ok();
        }
    }
}