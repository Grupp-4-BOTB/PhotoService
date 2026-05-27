using PhotoService.Domain.Entities;
using PhotoService.Domain.ValueObjects;

namespace PhotoService.Domain.Repositories;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(PhotoId id, CancellationToken ct = default);
    Task<Photo?> GetByOwnerIdAsync(OwnerId ownderId, CancellationToken ct = default);
    Task AddAsync(Photo photo, CancellationToken ct = default);
    Task UpdateAsync(Photo photo, CancellationToken ct = default);
    Task DeleteAsync(Photo photo, CancellationToken ct = default);   
}
