using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.ShowTimeAggregate
{
    public sealed class TicketId : AggregateRootId<Guid>
    {
        private TicketId(Guid value) : base(value)
        {
        }

        public static TicketId CreateUnique()
        {
            return new TicketId(Guid.NewGuid());
        }

        public static TicketId Create(Guid value)
        {
            return new TicketId(value);
        }
    }
}