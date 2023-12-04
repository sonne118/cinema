using ApiApplication.Domain.TicketAggregate;
using ErrorOr;
using MediatR;
namespace BuberDinner.Application.Dinners;

public record CreateReservationCommand(
    string HostId,
    string Name,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    bool IsPublic,
    int MaxGuests,
    DinnerPriceCommand Price,
    string MenuId,
    Uri ImageUrl,
    DinnerLocationCommand Location) : IRequest<ErrorOr<Reservation>>;

public record DinnerPriceCommand(
    decimal Amount,
    string Currency);

public record DinnerLocationCommand(
    string Name,
    string Address,
    double Latitude,
    double Longitude);