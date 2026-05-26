using Microsoft.EntityFrameworkCore;
using PhotoService.Domain.Entities;
using PhotoService.Domain.Repositories;
using PhotoService.Domain.ValueObjects;

namespace PhotoService.Infrastructure.Persistance.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly PhotoServiceDbContext _context;

    public PhotoRepository(PhotoServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Photo?> GetByIdAsync(PhotoId id, CancellationToken ct = default)
    {
        return await _context.Photos
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Photo?> GetByOwnerIdAsync(OwnerId ownerId, CancellationToken ct = default)
    {
        return await _context.Photos
            .FirstOrDefaultAsync(p => p.OwnerId == ownerId, ct);
    }

    public async Task AddAsync(Photo photo, CancellationToken ct = default)
    {
        await _context.Photos.AddAsync(photo, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Photo photo, CancellationToken ct = default)
    {
        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync(ct);
    }
}
