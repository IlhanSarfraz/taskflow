using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTaskCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{taskId:guid}/move")]
        public async Task<IActionResult> Move(
            Guid taskId,
            MoveTaskRequest request)
        {
            await _mediator.Send(
                new MoveTaskCommand(
                    taskId,
                    request.TargetColumnId));

            return NoContent();
        }
    }
}
