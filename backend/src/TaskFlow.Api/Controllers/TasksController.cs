using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Tasks.Commands.AssignTask;
using TaskFlow.Application.Features.Tasks.Commands.CreateComment;
using TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using TaskFlow.Application.Features.Tasks.Commands.DeleteComment;
using TaskFlow.Application.Features.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;
using TaskFlow.Application.Features.Tasks.Commands.ReorderTasks;
using TaskFlow.Application.Features.Tasks.Commands.UpdateComment;
using TaskFlow.Application.Features.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Features.Tasks.Commands.UploadAttachment;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Application.Features.Tasks.Queries.DownloadAttachment;
using TaskFlow.Application.Features.Tasks.Queries.GetMyTasks;
using TaskFlow.Application.Features.Tasks.Queries.GetProjectTasks;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskAttachments;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskComments;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskDetailPage;
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

        [HttpPut("taskId={taskId:guid}/move")]
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

        [HttpGet("taskId={taskId:guid}/detail")]
        public async Task<IActionResult> GetDetailPage(Guid taskId)
        {
            return Ok(await _mediator.Send(new GetTaskDetailPageQuery(taskId)));
        }

        [HttpGet("taskId={taskId:guid}")]
        public async Task<IActionResult> GetById(Guid taskId)
        {
            return Ok(await _mediator.Send(new GetTaskByIdQuery(taskId)));
        }

        [HttpPut("taskId={taskId:guid}")]
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

        [HttpDelete("taskId={taskId:guid}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            await _mediator.Send(
                new DeleteTaskCommand(taskId));

            return NoContent();
        }

        [HttpPut("taskId={taskId:guid}/assign")]
        public async Task<IActionResult> Assign(
            Guid taskId,
            AssignTaskRequest request)
        {
            await _mediator.Send(
                new AssignTaskCommand(taskId, request.AssigneeIds));

            return NoContent();
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            return Ok(await _mediator.Send(new GetMyTasksQuery()));
        }

        [HttpGet("/api/projects/projectId={projectId:guid}/tasks")]
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

        [HttpPost("taskId={taskId:guid}/comments")]
        public async Task<IActionResult> CreateComment(
            Guid taskId,
            CreateCommentRequest request)
        {
            return Ok(await _mediator.Send(
                new CreateCommentCommand(
                    taskId,
                    request.Content)));
        }

        [HttpGet("taskId={taskId:guid}/comments")]
        public async Task<IActionResult> GetComments(
            Guid taskId)
        {
            return Ok(await _mediator.Send(
                new GetTaskCommentsQuery(taskId)));
        }

        [HttpDelete("comments/commentId={commentId:guid}")]
        public async Task<IActionResult> DeleteComment(
            Guid commentId)
        {
            await _mediator.Send(
                new DeleteCommentCommand(commentId));

            return NoContent();
        }

        [HttpPut("comments/commentId={commentId:guid}")]
        public async Task<IActionResult> UpdateComment(
            Guid commentId,
            UpdateCommentRequest request)
        {
            await _mediator.Send(
                new UpdateCommentCommand(
                    commentId,
                    request.Content));

            return NoContent();
        }

        [HttpPut("/api/columns/columnId={columnId:guid}/tasks/reorder")]
        public async Task<IActionResult> ReorderTasks(
            Guid columnId,
            ReorderTasksCommand command)
        {
            await _mediator.Send(command with { ColumnId = columnId });
            return NoContent();
        }

        [HttpPost("{taskId:guid}/attachments")]
        public async Task<ActionResult<Guid>> UploadAttachment(
            Guid taskId,
            IFormFile file)
        {
            return Ok(await _mediator.Send(
                new UploadAttachmentCommand(taskId, file)));
        }

        [HttpGet("{taskId:guid}/attachments")]
        public async Task<ActionResult<List<TaskAttachmentDto>>> GetAttachments(
            Guid taskId)
        {
            return Ok(await _mediator.Send(
                new GetTaskAttachmentsQuery(taskId)));
        }

        [HttpGet("attachments/{attachmentId:guid}/download")]
        public async Task<IActionResult> DownloadAttachment(
            Guid attachmentId)
        {
            DownloadAttachmentResponse response =
                await _mediator.Send(
                    new DownloadAttachmentQuery(attachmentId));

            return File(
                response.Stream,
                response.ContentType,
                response.FileName);
        }
    }
}
