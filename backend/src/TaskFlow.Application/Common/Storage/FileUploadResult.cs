namespace TaskFlow.Application.Common.Storage
{
    public sealed record FileUploadResult(
        string PublicId,
        string Url,
        long Bytes);
}
