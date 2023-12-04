using ApiApplication.Domain.Common.Models;
using ErrorOr;

namespace ApiApplication.Domain.ShowTimeAggregate
{

    public sealed class AuditoriumId : EntityId<Guid>
    {
        public AuditoriumId(Guid value) : base(value)
        {
        }

        public static AuditoriumId CreateUnique()
        {
            // TODO: enforce invariants
            return new AuditoriumId(Guid.NewGuid());
        }

        //public static AuditoriumId Create(Guid value)
        //{
        //    // TODO: enforce invariants
        //    return new AuditoriumId(value);
        //}

        public static ErrorOr<AuditoriumId> Create(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                return Common.DomainErrors.Errors.Auditorium.InvalidMenuId;
            }

            return new AuditoriumId(guid);
        }
    }

}