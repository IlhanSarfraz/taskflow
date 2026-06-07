using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Auth.Commands.CreateProject;

namespace TaskFlow.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(
            CreateProjectCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
