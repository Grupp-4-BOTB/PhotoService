using Microsoft.Extensions.Options;

namespace PhotoService.Presentation.API.Security;

public sealed class ApiKeyEndpointFilter(IOptions<ApiKeyOption> options) : IEndpointFilter
{

    private readonly ApiKeyOption _options = options.Value;
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if(string.IsNullOrWhiteSpace(_options.Value))
            return ValueTask.FromResult<object?>(Results.Unauthorized());

        if (!context.HttpContext.Request.Headers.TryGetValue(_options.HeaderName, out var providedApiKey))
            return ValueTask.FromResult<object?>(Results.Unauthorized());
       
        if(!string.Equals(providedApiKey, _options.Value, StringComparison.Ordinal))
            return ValueTask.FromResult<object?>(Results.Unauthorized());

        return next(context);
    }
}
