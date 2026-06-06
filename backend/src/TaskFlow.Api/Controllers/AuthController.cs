using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Auth.Commands.Login;
using TaskFlow.Application.Features.Auth.Commands.RefreshToken;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        return Ok(await _mediator.Send(new RefreshTokenCommand()));
    }
}
