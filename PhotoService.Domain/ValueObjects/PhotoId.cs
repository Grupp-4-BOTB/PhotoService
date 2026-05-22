using PhotoService.Domain.Exceptions;

namespace PhotoService.Domain.ValueObjects;

public record PhotoId
{
    public Guid Value { get; }
    public PhotoId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("PhotoId cannot be empty.");
        Value = value;
    }
    public static PhotoId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
