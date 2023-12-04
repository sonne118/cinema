using ApiApplication.Domain.Common.Models;
using System.Net;
using System.Xml.Linq;

namespace ApiApplication.Domain.ShowTimeAggregate;

public sealed class SeatEntity : Entity<SeatId> //: ValueObject
{
    public short Row { get; set; }
    public short SeatNumber { get; set; }
    public int AuditoriumId { get; set; }
    public Auditorium Auditorium { get; set; }

    public SeatEntity(short row, short seatNumber, int auditoriumId, Auditorium auditorium)
    {
        Row = row;
        SeatNumber = seatNumber;
        AuditoriumId = auditoriumId;
        Auditorium = auditorium;
    }

    public static SeatEntity Create(short row, short seatNumber, int auditoriumId, Auditorium auditorium)
    {
        // TODO: Enforce invariants
        return new SeatEntity(row, seatNumber, auditoriumId, auditorium);
    }

    //public override IEnumerable<object> GetEqualityComponents()
    //{
    //    yield return Row;
    //    yield return SeatNumber;
    //    yield return AuditoriumId;      
    //}
}