using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Auth.Commands.Register;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
