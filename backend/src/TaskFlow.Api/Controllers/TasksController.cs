using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;
using TaskFlow.Application.Features.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

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

        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetById(Guid taskId)
        {
            return Ok(await _mediator.Send(new GetTaskByIdQuery(taskId)));
        }

        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> Update(
            Guid taskId,
            UpdateTaskRequest request)
        {
            await _mediator.Send(
                new UpdateTaskCommand(
                    taskId,
                    request.Title,
                    request.Description,
                    request.Priority,
                    request.DueDate));

            return NoContent();
        }
    }
}
