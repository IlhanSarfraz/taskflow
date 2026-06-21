using TaskFlow.Application.Features.Projects.DTOs;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Dtos;

public sealed record TaskDetailPageResponse(
    TaskDetailsResponse Task,
    IReadOnlyList<CommentResponse> Comments,
    IReadOnlyList<ProjectMemberResponse> Members);
