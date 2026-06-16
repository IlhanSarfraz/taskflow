namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed record CommentResponse(
        Guid Id,
        Guid UserId,
        string UserName,
        string Content,
        DateTime CreatedAt);
}