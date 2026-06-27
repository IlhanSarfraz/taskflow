using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Common.Storage;

namespace TaskFlow.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default);
}