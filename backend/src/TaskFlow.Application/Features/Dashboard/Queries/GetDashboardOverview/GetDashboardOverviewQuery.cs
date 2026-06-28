using MediatR;
using TaskFlow.Application.Features.Dashboard.Dtos;

namespace TaskFlow.Application.Features.Dashboard.Queries.GetDashboardOverview;

public sealed record GetDashboardOverviewQuery : IRequest<DashboardOverviewDto>;