using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Tasks.Commands.AssignTask;
using TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using TaskFlow.Application.Features.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;
using TaskFlow.Application.Features.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Application.Features.Tasks.Queries.GetMyTasks;
using TaskFlow.Application.Features.Tasks.Queries.GetProjectTasks;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using TaskFlow.Domain.Enums;

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

        [HttpDelete("{taskId:guid}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            await _mediator.Send(
                new DeleteTaskCommand(taskId));

            return NoContent();
        }

        [HttpPut("{taskId:guid}/assign")]
        public async Task<IActionResult> Assign(
            Guid taskId,
            AssignTaskRequest request)
        {
            await _mediator.Send(
                new AssignTaskCommand(taskId, request.AssigneeId));

            return NoContent();
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            return Ok(await _mediator.Send(new GetMyTasksQuery()));
        }

        [HttpGet("/api/projects/{projectId:guid}/tasks")]
        public async Task<IActionResult> GetProjectTasks(
            Guid projectId,
            Guid? columnId,
            TaskPriority? priority,
            int page = 1,
            int pageSize = 20)
        {
            return Ok(await _mediator.Send(
                new GetProjectTasksQuery(
                    projectId,
                    columnId,
                    priority,
                    page,
                    pageSize)));
        }
    }
}
