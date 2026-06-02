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
        _containerName = configuration["BlobStorageContainer:ImageContainerName"]
            ?? throw new InvalidOperationException("BlobStorageContainer:ImageContainerName is missing");
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

    public async Task<Photo> ReplaceAsync(Guid id, IFormFile file, CancellationToken ct = default)
    {
        var photoId = new PhotoId(id);
        var photo = await _repository.GetByIdAsync(photoId, ct);

        if (photo is null)
            throw new KeyNotFoundException($"Photo with id {id} was not found.");

        var containerClient= _blobServiceClient.GetBlobContainerClient(_containerName);
        var oldBlobClient = containerClient.GetBlobClient(photo.FileName);
        await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
        // denna delen raderar den gamla filen från blob storage.

        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var newBlobClient = containerClient.GetBlobClient(uniqueFileName);
        await using var stream = file.OpenReadStream();

        await newBlobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                CacheControl = "public, max-age=31536000"
            }
        }, cancellationToken: ct);
        // denna delen laddar upp den nya filen till blob storage.

        photo.Update(uniqueFileName, file.ContentType, file.Length, newBlobClient.Uri.ToString());
        await _repository.UpdateAsync(photo, ct);
        // denna delen uppdaterar photo entiteten i databasen.

        return photo;
        //Sammanfattning: metoden tar bort den gamla filen från min blobstorage, laddare upp den nya filen och uppdaterar photo entiteten i databasen med den nya filens info.
    }

    public async Task<Photo?> GetByOwnerIdAsync(string ownerId, CancellationToken ct = default)
    {
        var owner = new OwnerId(ownerId);
        return await _repository.GetByOwnerIdAsync(owner, ct);
    }
}
