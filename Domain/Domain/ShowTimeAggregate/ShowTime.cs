using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.ShowTimeAggregate
{
    public sealed class ShowTime : AggregateRoot<ShowTimeId, Guid>
    {
        //public int Id { get; set; } Guid
        public ShowTimeId ShowTimeId { get; private set; }
        public DateTime SessionDate { get; private set; }
        public MovieId MovieId { get; private set; }
        public AuditoriumId AuditoriumId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime UpdatedDateTime { get; private set; }

        private readonly List<TicketEntity> _tickets = new();
        public IReadOnlyList<TicketEntity> TicketEntities => _tickets.AsReadOnly();
        private ShowTime(
            ShowTimeId showTimeId,
            MovieId movieId,
            DateTime sessionDate,
            AuditoriumId auditoriumId)
            : base(showTimeId)
        {
            MovieId = movieId;
            AuditoriumId = auditoriumId;
            SessionDate = sessionDate;
            ShowTimeId = showTimeId;
        }

        public static ShowTime Create(
            MovieId movieId,
            DateTime sessionDate,
            AuditoriumId auditoriumId)
        {
            // TODO: enforce invariants
            var showTime = new ShowTime(
                ShowTimeId.CreateUnique(),
                movieId,
                sessionDate,
                auditoriumId);

            showTime.AddDomainEvent(new ShowTimeCreated(showTime));

            return showTime;
        }

        public void AddTicket(TicketEntity ticket)
        {
            _tickets.Add(ticket);
        }

        private ShowTime()
        {
        }
    }

}