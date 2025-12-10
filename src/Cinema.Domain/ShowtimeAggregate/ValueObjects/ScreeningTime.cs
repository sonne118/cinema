using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ShowtimeAggregate.ValueObjects;




public sealed class ScreeningTime : ValueObject
{
    public DateTime Value { get; private set; }

    private ScreeningTime(DateTime value)
    {
        Value = value;
    }

    public static ScreeningTime Create(DateTime value)
    {
        
        if (value <= DateTime.UtcNow)
            throw new ArgumentException("Screening time must be in the future", nameof(value));

        return new ScreeningTime(value);
    }

    public static ScreeningTime CreateForTesting(DateTime value)
    {
        
        return new ScreeningTime(value);
    }

    public bool HasPassed()
    {
        return Value < DateTime.UtcNow;
    }

    public bool IsWithinHours(int hours)
    {
        return Value <= DateTime.UtcNow.AddHours(hours);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");

#pragma warning disable CS8618
    private ScreeningTime() { }
#pragma warning restore CS8618
}
