using ApiApplication.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Menus.Queries.ListMenus;

public record CreateShowTimeCommand(
    //string HostId,
    //string Name,
    //string Description,
      Guid MovieId,
      string sessionDate,
      string auditoriumId,

    List<CreateShowTimeTicketCommand> Sections) : IRequest<ErrorOr<ShowTime>>;

public record CreateShowTimeTicketCommand(

      string CreatedTime,
      bool Paid,
      string ShowTimeId,
List<CreateShowTimeSeatCommand> Items);

public record CreateShowTimeSeatCommand(
    //string Name,
    //string Description
     short Row,
     short SeatNumber,
     int AuditoriumId);