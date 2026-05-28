namespace PhotoService.Presentation.API.Dtos;

public sealed record UploadImageResult 
(
    bool Succeeded,
    Guid? Id,
    string? FileName,
    string? Url,
    string? ContentType,
    long Size
)
{
    public static UploadImageResult Success(Guid id,string fileName, string url, string contentType, long size)
        => new(true, id, fileName, url, contentType, size);

    public static UploadImageResult Failed ()
        => new(false, null, null, null, null, 0);
}
