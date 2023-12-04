using Ardalis.SmartEnum;

namespace BuberDinner.Domain.DinnerAggregate.Enums;

public class TicketStatus : SmartEnum<TicketStatus>
{
    public TicketStatus(string name, int value) : base(name, value)
    {
    }

    public static readonly TicketStatus Upcoming = new(nameof(Upcoming), 1);
    public static readonly TicketStatus InProgress = new(nameof(InProgress), 2);
    public static readonly TicketStatus Ended = new(nameof(Ended), 3);
    public static readonly TicketStatus Cancelled = new(nameof(Cancelled), 4);
}