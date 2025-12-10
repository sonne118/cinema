using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ShowtimeAggregate.ValueObjects;




public sealed class ShowtimeId : ValueObject
{
    public Guid Value { get; private set; }

    private ShowtimeId(Guid value)
    {
        Value = value;
    }

    public static ShowtimeId CreateUnique()
    {
        return new ShowtimeId(Guid.NewGuid());
    }

    public static ShowtimeId Create(Guid value)
    {
        return new ShowtimeId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

#pragma warning disable CS8618
    private ShowtimeId() { }
#pragma warning restore CS8618
}
