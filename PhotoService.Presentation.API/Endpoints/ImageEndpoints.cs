using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PhotoService.Presentation.API.Dtos;

namespace PhotoService.Presentation.API.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/images")
            .WithTags("Images")
            .WithDescription("Upload images to storage.");

        group.MapPost("/upload", UploadImageAsync)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadImageResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> UploadImageAsync(IFormFile file, BlobServiceClient client, IConfiguration config, CancellationToken ct)
    {
        if (file.Length == 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = ["A file must be provided."]
            });

        var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "image/png",
        };

        if(!allowedContentTypes.Contains(file.ContentType))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = ["Only JPEG, JPG, WEBP and PNG images are allowed."]
            });

        var containerName = config["BlobStorageContainer:ImageContainerName"]
            ?? throw new InvalidOperationException("BlobStorageContainer:ImageContainerName is missing.");

        var containerClient = client.GetBlobContainerClient(containerName);
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

        var result = UploadImageResult.Success
            (
                uniqueFileName,
                blobClient.Uri.ToString(),
                file.ContentType,
                file.Length
            );

        return Results.Created(result.Url, result);
    }
}
