using Ardalis.SmartEnum;

namespace ApiApplication.Domain.TicketAggregate
{
    public class ReservationStatus : SmartEnum<ReservationStatus>
    {
        public ReservationStatus(string name, int value) : base(name, value)
        {
        }
        public static readonly ReservationStatus Reserved = new(nameof(Reserved), 1);
        public static readonly ReservationStatus Cancelled = new(nameof(Cancelled), 2);
    }

}