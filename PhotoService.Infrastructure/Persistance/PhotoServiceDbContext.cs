using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PhotoService.Domain.Entities;

namespace PhotoService.Infrastructure.Persistance;

public class PhotoServiceDbContext : DbContext
{
    public PhotoServiceDbContext(DbContextOptions<PhotoServiceDbContext> options) : base(options)
    {
        
    }

    public DbSet<Photo>
}
