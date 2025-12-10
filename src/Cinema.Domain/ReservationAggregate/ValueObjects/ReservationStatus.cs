using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ReservationAggregate.ValueObjects;




public sealed class ReservationStatus : ValueObject
{
    public ReservationStatusType Value { get; private set; }

    private ReservationStatus(ReservationStatusType value)
    {
        Value = value;
    }

    public static ReservationStatus Pending => new(ReservationStatusType.Pending);
    public static ReservationStatus Confirmed => new(ReservationStatusType.Confirmed);
    public static ReservationStatus Cancelled => new(ReservationStatusType.Cancelled);
    public static ReservationStatus Expired => new(ReservationStatusType.Expired);

    public bool IsPending => Value == ReservationStatusType.Pending;
    public bool IsConfirmed => Value == ReservationStatusType.Confirmed;
    public bool IsCancelled => Value == ReservationStatusType.Cancelled;
    public bool IsExpired => Value == ReservationStatusType.Expired;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

#pragma warning disable CS8618
    private ReservationStatus() { }
#pragma warning restore CS8618
}

public enum ReservationStatusType
{
    Pending,
    Confirmed,
    Cancelled,
    Expired
}
