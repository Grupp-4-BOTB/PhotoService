using PhotoService.Domain.Exceptions;
using PhotoService.Domain.ValueObjects;

namespace PhotoService.Domain.Entities;

public class Photo
{
    public PhotoId Id { get; private set; }
    public OwnerId OwnerId { get; private set; }
    public string Url { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long SizeInBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private static readonly string[] AllowedContentTypes =
        ["image/jpeg", "image/png", "image/webp"];

    private static readonly long MaxSizeInBytes = 5 * 1024 * 1024;

    private Photo() { }

    public static Photo Create (
        OwnerId ownerId,
        string fileName,
        string contentType,
        long sizeInBytes,
        string url)
    {
        if (!AllowedContentTypes.Contains(contentType))
            throw new DomainException("Only JPEG, PNG, and WEBP formats are allowed.");

        if (sizeInBytes > MaxSizeInBytes)
            throw new DomainException("File size cannot exceed 5 MB.");

        if(string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty.");

        if(string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL cannot be empty.");

        return new Photo
        {
            Id = PhotoId.New(),
            OwnerId = ownerId,
            FileName = fileName.Trim(),
            ContentType = contentType,
            SizeInBytes = sizeInBytes,
            Url = url,
            UploadedAt = DateTime.UtcNow
        };
    }

    public void Update(string fileName, string contentType, long sizeInBytes, string url)
    {
        if (!AllowedContentTypes.Contains(contentType))
            throw new DomainException("Only JPEG, PNG, and WEBP formats are allowed.");

        if(string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL cannot be empty.");

        if (sizeInBytes > MaxSizeInBytes)
            throw new DomainException("File size cannot exceed 5 MB.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty.");

        FileName = fileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        Url = url;
    }
}
