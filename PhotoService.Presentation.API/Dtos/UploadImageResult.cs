namespace PhotoService.Presentation.API.Dtos;

public sealed record UploadImageResult 
(
    bool Succeeded,
    string? FileName,
    string? Url,
    string? ContentType,
    long Size
)
{
    public static UploadImageResult Success(string fileName, string url, string contentType, long size)
        => new(true, fileName, url, contentType, size);

    public static UploadImageResult Failed ()
        => new(false, null, null, null, 0);
}
