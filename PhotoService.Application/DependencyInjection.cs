using Microsoft.Extensions.DependencyInjection;
using PhotoService.Application.Services;

namespace PhotoService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PhotoAppService>();
        return services;
    }
}
