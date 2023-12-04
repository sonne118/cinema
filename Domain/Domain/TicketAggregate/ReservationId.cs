using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.TicketAggregate
{
    public sealed class ReservationId : AggregateRootId<Guid>
    {
        private ReservationId(Guid value) : base(value)
        {
        }

        public static ReservationId CreateUnique()
        {
            return new ReservationId(Guid.NewGuid());
        }

        public static ReservationId Create(Guid value)
        {
            return new ReservationId(value);
        }
    }
}