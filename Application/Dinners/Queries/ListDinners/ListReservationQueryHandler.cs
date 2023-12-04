using ApiApplication.Domain.TicketAggregate;
using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Dinners.Queries.ListDinners;

public class ListReservationQueryHandler : IRequestHandler<ListReservationQuery, ErrorOr<List<Reservation>>>
{
    private readonly IReservationRepository _dinnerRepository;

    public ListReservationQueryHandler(IReservationRepository dinnerRepository)
    {
        _dinnerRepository = dinnerRepository;
    }

    public async Task<ErrorOr<List<Reservation>>> Handle(ListReservationQuery request, CancellationToken cancellationToken)
    {
        // var hostId = HostId.Create(request.HostId);
        // return await _dinnerRepository.ListAsync(hostId);
        return null;
    }
}