using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate;

public sealed class SeatId : EntityId<Guid>
    {
        private SeatId(Guid value) : base(value)
        {
        }

        public static SeatId Create(Guid value)
        {
            return new SeatId(value);
        }

        public static SeatId CreateUnique()
        {
            return new SeatId(Guid.NewGuid());
        }

    private SeatId()
    {
            
    }
}
