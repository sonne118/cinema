using ApiApplication.Domain.TicketAggregate;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Dinners.Queries.ListDinners;

public record ListReservationQuery(string HostId) : IRequest<ErrorOr<List<Reservation>>>;