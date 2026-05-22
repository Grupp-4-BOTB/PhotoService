using PhotoService.Domain.Exceptions;

namespace PhotoService.Domain.ValueObjects;

public record OwnerId
{
    public string Value { get; }
    public OwnerId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("OwnerId cannot be null or empty.");

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
