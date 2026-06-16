namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed class CreateCommentRequest
    {
        public string Content { get; set; }
            = string.Empty;
    }
}
