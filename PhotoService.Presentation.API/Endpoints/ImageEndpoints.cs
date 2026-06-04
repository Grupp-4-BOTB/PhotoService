using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PhotoService.Application.Services;
using PhotoService.Domain.Exceptions;
using PhotoService.Domain.ValueObjects;
using PhotoService.Presentation.API.Dtos;
using PhotoService.Presentation.API.Security;

namespace PhotoService.Presentation.API.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/images")
            .WithTags("Images")
            .WithDescription("Upload images to storage.")
            .AddEndpointFilter<ApiKeyEndpointFilter>();

        group.MapPost("/upload", UploadImageAsync)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadImageResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", ReplaceAsync)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);


        group.MapGet("/{id:guid}", GetByIdAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/owner/{ownerId}", GetByOwnerIdAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    }

    private static async Task<IResult> UploadImageAsync(IFormFile file, PhotoAppService service, CancellationToken ct)
    {
        if (file.Length == 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = ["A file must be provided"]
            });

        var ownerId = new OwnerId("test-user");

        try
        {
            var photo = await service.UploadAsync(ownerId, file, ct);
            var result = UploadImageResult.Success(photo.Id.Value, photo.FileName, photo.Url, photo.ContentType, photo.SizeInBytes);
            return Results.Created(result.Url!, result);
        }
        catch(DomainException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = [ex.Message]
            });
        }
    }

    private static async Task<IResult> GetByIdAsync(Guid id, PhotoAppService service, CancellationToken ct)
    {
        var photo = await service.GetByIdAsync(id, ct);

        if (photo is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(photo);
    }

    private static async Task<IResult> DeleteAsync(Guid id, PhotoAppService service, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> ReplaceAsync(Guid id, IFormFile file, PhotoAppService service, CancellationToken ct)
    {
        try
        {
            var photo = await service.ReplaceAsync(id, file, ct);
            return Results.Ok(photo);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetByOwnerIdAsync(string ownerId, PhotoAppService service, CancellationToken ct)
    {
        var photo = await service.GetByOwnerIdAsync(ownerId, ct);

        if (photo is null)
            return Results.NotFound();

        return Results.Ok(photo);
    }
}
