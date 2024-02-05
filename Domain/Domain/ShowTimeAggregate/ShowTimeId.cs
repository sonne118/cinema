using ApiApplication.Domain.Common.DomainErrors;
using ApiApplication.Domain.Common.Models;
using ErrorOr;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{
    public sealed class ShowTimeId : AggregateRootId<Guid>
    {
        private ShowTimeId(Guid value) : base(value)
        {
        }

        public static ShowTimeId CreateUnique()
        {
            // TODO: enforce invariants
            return new ShowTimeId(Guid.NewGuid());
        }

        public static ShowTimeId Create(Guid value)
        {
            // TODO: enforce invariants
            return new ShowTimeId(value);
        }

        public static ErrorOr<ShowTimeId> Create(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                return Errors.ShowTimes.InvalidMenuId;
            }

            return new ShowTimeId(guid);
        }       

    }
}