using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Dashboard.Queries.GetDashboardOverview;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-dashboard")]
    public async Task<IActionResult> GetDashboardOverview()
    {
        return Ok(await _mediator.Send(new GetDashboardOverviewQuery()));
    }
}
