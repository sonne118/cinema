using ApiApplication.Domain.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

namespace ApiApplication.Application.ShowTimes.Command.CreateShowTime;

public record CreateShowTimeCommand(
    //string HostId,
    //string Name,
    //string Description,
      string MovieId,
      string sessionDate,
      string auditoriumId

    //List<CreateShowTimeTicketCommand> Tickets
    ) : IRequest<ErrorOr<ShowTime>>;

public record CreateShowTimeTicketCommand(

      string CreatedTime,
      bool Paid,
      string ShowTimeId,
List<CreateShowTimeSeatCommand> Seats);

public record CreateShowTimeSeatCommand(
    //string Name,
    //string Description
     short Row,
     short SeatNumber,
     Guid AuditoriumId);