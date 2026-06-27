using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Storage;
using TaskFlow.Infrastructure.Options;

namespace TaskFlow.Infrastructure.Services;

public sealed class CloudinaryFileStorageService
    : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(
        IOptions<CloudinaryOptions> options)
    {
        CloudinaryOptions settings = options.Value;

        Account account = new(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<FileUploadResult> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using Stream stream = file.OpenReadStream();

        RawUploadParams uploadParams = new()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "taskflow"
        };

        RawUploadResult result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return new FileUploadResult(
            PublicId: result.PublicId,
            Url: result.SecureUrl.ToString(),
            Bytes: result.Bytes);
    }

    public async Task DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        DeletionParams deleteParams = new(publicId)
        {
            ResourceType = ResourceType.Raw
        };

        DeletionResult result =
            await _cloudinary.DestroyAsync(deleteParams);

        if (result.Result != "ok")
            throw new Exception("Unable to delete attachment.");
    }
}