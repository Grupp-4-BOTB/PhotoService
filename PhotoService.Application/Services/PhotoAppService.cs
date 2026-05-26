using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PhotoService.Domain.Entities;
using PhotoService.Domain.Repositories;
using PhotoService.Domain.ValueObjects;

namespace PhotoService.Application.Services;

public class PhotoAppService
{
    private readonly IPhotoRepository _repository;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public PhotoAppService(IPhotoRepository repository, BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _repository = repository;
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["AzureBlobStorage:ImageContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ImageContainerName is missing");
    }

    public async Task<Photo> UploadAsync(OwnerId ownerId, IFormFile)
    public async Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var photoId = new PhotoId(id);
        return await _repository.GetByIdAsync(photoId, ct);
    }

    public async Task AddAsync(Photo photo, CancellationToken ct = default)
    {
        await _repository.AddAsync(photo, ct);
    }

    public async Task DeleteAsync (Guid id, CancellationToken ct = default)
    {
        var photoId = new PhotoId(id);
        var photo = await _repository.GetByIdAsync(photoId, ct);

        if (photo is null)
            throw new KeyNotFoundException($"Photo with id {id} was not found.");

        await _repository.DeleteAsync(photo, ct);
    }
}
