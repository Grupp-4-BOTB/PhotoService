using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotoService.Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using PhotoService.Domain.Repositories;
using PhotoService.Infrastructure.Persistance.Repositories;

namespace PhotoService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhotoServiceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPhotoRepository, PhotoRepository>();

        return services;
    }
}
