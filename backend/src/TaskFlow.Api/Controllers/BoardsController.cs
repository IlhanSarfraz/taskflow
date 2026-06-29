using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Boards.Commands.CreateBoard;
using TaskFlow.Application.Features.Boards.Commands.CreateColumn;
using TaskFlow.Application.Features.Boards.Commands.DeleteColumn;
using TaskFlow.Application.Features.Boards.Commands.RenameColumn;
using TaskFlow.Application.Features.Boards.Commands.ReorderColumns;
using TaskFlow.Application.Features.Boards.Commands.SetDoneColumn;
using TaskFlow.Application.Features.Boards.Commands.UnsetDoneColumn;
using TaskFlow.Application.Features.Boards.Queries.GetBoardById;
using TaskFlow.Application.Features.Boards.Queries.GetBoardsByProject;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class BoardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BoardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBoardCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("/api/projects/{projectId:guid}/boards")]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            return Ok(await _mediator.Send(
                new GetBoardsByProjectQuery(projectId)));
        }

        [HttpGet("{boardId:guid}")]
        public async Task<IActionResult> GetById(Guid boardId)
        {
            return Ok(await _mediator.Send(
                new GetBoardByIdQuery(boardId)));
        }

        // ---------------- COLUMNS ----------------

        [HttpPost("{boardId:guid}/columns")]
        public async Task<IActionResult> CreateColumn(
            Guid boardId,
            CreateColumnCommand command)
        {
            if (boardId != command.BoardId)
                return BadRequest("BoardId mismatch");

            return Ok(await _mediator.Send(command));
        }

        [HttpPut("columns/{columnId:guid}")]
        public async Task<IActionResult> RenameColumn(
            Guid columnId,
            RenameColumnCommand command)
        {
            if (columnId != command.ColumnId)
                return BadRequest("ColumnId mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("columns/{columnId:guid}")]
        public async Task<IActionResult> DeleteColumn(Guid columnId)
        {
            await _mediator.Send(
                new DeleteColumnCommand(columnId));

            return NoContent();
        }

        [HttpPut("{boardId:guid}/columns/reorder")]
        public async Task<IActionResult> ReorderColumns(
            Guid boardId,
            ReorderColumnsCommand command)
        {
            if (boardId != command.BoardId)
                return BadRequest("BoardId mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{boardId:guid}/done-column/{columnId:guid}")]
        public async Task<IActionResult> SetDoneColumn(
            Guid boardId,
            Guid columnId)
        {
            await _mediator.Send(
                new SetDoneColumnCommand(boardId, columnId));

            return NoContent();
        }

        [HttpPatch("{boardId:guid}/done-column/unset")]
        public async Task<IActionResult> UnsetDoneColumn(Guid boardId)
        {
            await _mediator.Send(new UnsetDoneColumnCommand(boardId));

            return NoContent();
        }
    }
}
