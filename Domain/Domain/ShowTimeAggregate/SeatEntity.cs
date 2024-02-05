using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate;

public sealed class SeatEntity : Entity<SeatId> //: ValueObject
{
   // public Guid Id { get; set; }
    public short Row { get; set; }
    public short SeatNumber { get; set; }

    // public TicketId TicketId { get; set; }
     public AuditoriumId AuditoriumId { get; set; }
    public Auditorium Auditorium { get; set; }
   // public AuditoriumId AuditoriumId { get; private set; }

    public SeatEntity(short row, short seatNumber, Auditorium auditorium, AuditoriumId auditoriumId) // int auditoriumId
     : base(SeatId.CreateUnique())
    {
        Row = row;
        SeatNumber = seatNumber;
        AuditoriumId = auditoriumId;
        Auditorium = auditorium;
    }

    public static SeatEntity Create(short row, short seatNumber, AuditoriumId auditoriumId, Auditorium auditorium)   //int auditoriumId
    {
        // TODO: Enforce invariants
        return new SeatEntity(row, seatNumber, auditorium, auditoriumId);
    }

    private SeatEntity()
    {
        
    }

    //public override IEnumerable<object> GetEqualityComponents()
    //{
    //    yield return Row;
    //    yield return SeatNumber;
    //    yield return AuditoriumId;      
    //}
}