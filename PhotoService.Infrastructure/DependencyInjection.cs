using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhotoService.Domain.Repositories;
using PhotoService.Infrastructure.Persistance;
using PhotoService.Infrastructure.Persistance.Repositories;

namespace PhotoService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhotoServiceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPhotoRepository, PhotoRepository>();

        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage")
                ?? throw new InvalidOperationException("AzureBlobStorage conntectionString is missing");

            return new BlobServiceClient(connectionString);
        });

        return services;
    }
}
