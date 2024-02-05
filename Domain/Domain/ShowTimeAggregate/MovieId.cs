using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{

    public sealed class MovieId : EntityId<Guid>
    {
        private MovieId(Guid value) : base(value)
        {
        }

        public static MovieId Create(Guid value)
        {
            return new MovieId(value);
        }

        public static MovieId CreateUnique()
        {
            return new MovieId(Guid.NewGuid());
        }

        private MovieId()
        {
                
        }
    }

}