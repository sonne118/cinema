using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{
    public sealed class TicketId : EntityId<Guid>//AggregateRootId<Guid>
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
        public TicketId()
        {
            
        }

    }
}