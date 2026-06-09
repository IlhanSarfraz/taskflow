using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Boards.Commands.CreateBoard;
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
    }
}
