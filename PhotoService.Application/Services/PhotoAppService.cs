using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

    public async Task<Photo> UploadAsync(OwnerId ownerId, IFormFile file, CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        var blobClient = containerClient.GetBlobClient(uniqueFileName);
        await using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                CacheControl = "public, max-age=31536000"
            }
        }, cancellationToken: ct);

        var photo = Photo.Create(ownerId, uniqueFileName, file.ContentType, file.Length, blobClient.Uri.ToString());
        await _repository.AddAsync(photo, ct);

        return photo;
    }
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
