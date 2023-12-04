using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.ShowTimeAggregate
{
    public sealed class Auditorium : Entity<AuditoriumId>
    {
        private readonly List<ShowTime> _items = new();

        private readonly List<SeatEntity> _seats = new();
        public string Name { get; private set; }
        public string Description { get; private set; }

        public IReadOnlyList<ShowTime> Items => _items.AsReadOnly();

        public IReadOnlyList<SeatEntity> Seats => _seats.AsReadOnly();

        private Auditorium(string name, string description, List<SeatEntity> seats, List<ShowTime> items, AuditoriumId id = null)
            : base(id ?? AuditoriumId.CreateUnique())
        {
            Name = name;
            Description = description;
            _items = items;
            _seats = seats;
        }

        public static Auditorium Create(
            string name,
            string description,
            List<SeatEntity> seats = null,
            List<ShowTime> items = null)
        {
            // TODO: enforce invariants
            return new Auditorium(name, description, seats ?? new (), items ?? new());
        }

        private Auditorium()
        {
        }
    }
}