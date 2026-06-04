namespace PhotoService.Presentation.API.Security;

public sealed class ApiKeyOption
{
    public const string SectionName = "ApiKey";
    public string HeaderName { get; init; } = "X-API-Key";
    public string Value { get; init; } = null!;
}
