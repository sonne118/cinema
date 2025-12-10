using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ReservationAggregate.ValueObjects;




public sealed class ReservationId : ValueObject
{
    public Guid Value { get; private set; }

    private ReservationId(Guid value)
    {
        Value = value;
    }

    public static ReservationId CreateUnique()
    {
        return new ReservationId(Guid.NewGuid());
    }

    public static ReservationId Create(Guid value)
    {
        return new ReservationId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

#pragma warning disable CS8618
    private ReservationId() { }
#pragma warning restore CS8618
}
