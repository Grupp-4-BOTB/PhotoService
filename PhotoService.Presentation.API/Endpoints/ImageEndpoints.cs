using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PhotoService.Application.Services;
using PhotoService.Domain.ValueObjects;
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

        group.MapGet("/{id:guid}", GetByIdAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteByIdAsync")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

    }

    private static async Task<IResult> UploadImageAsync(IFormFile file, PhotoAppService service, CancellationToken ct)
    {
        var ownerId = new OwnerId("test-user");

        var photo = await service.UploadAsync(ownerId, file, ct);

        return Results.Created($"/api/images/{photo.Id.Value}", photo);
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
}
