using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ReservationAggregate.ValueObjects;




public sealed class SeatNumber : ValueObject
{
    public int Row { get; private set; }
    public int Number { get; private set; }

    private SeatNumber(int row, int number)
    {
        Row = row;
        Number = number;
    }

    public static SeatNumber Create(int row, int number)
    {
        if (row <= 0)
            throw new ArgumentException("Row must be greater than 0", nameof(row));

        if (number <= 0)
            throw new ArgumentException("Seat number must be greater than 0", nameof(number));

        return new SeatNumber(row, number);
    }

    
    
    
    public bool IsContiguousWith(SeatNumber other)
    {
        return Row == other.Row && Math.Abs(Number - other.Number) == 1;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Row;
        yield return Number;
    }

    public override string ToString() => $"Row {Row}, Seat {Number}";

#pragma warning disable CS8618
    private SeatNumber() { }
#pragma warning restore CS8618
}
