using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Users.Commands.UpdateProfile;
using TaskFlow.Application.Features.Users.Queries.GetUserActivity;
using TaskFlow.Application.Features.Users.Queries.GetUserProfile;
using TaskFlow.Application.Features.Users.Queries.SearchUsers;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string search)
        {
            return Ok(await _mediator.Send(new SearchUsersQuery(search)));
        }

        [HttpGet("activity")]
        public async Task<IActionResult> GetActivity([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _mediator.Send(new GetUserActivityQuery(page, pageSize)));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return Ok(await _mediator.Send(new GetUserProfileQuery()));
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}