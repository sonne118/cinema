using ApiApplication.Domain.ShowTimeAggregate;
using ApiApplication.Domain.TicketAggregate;
using Application.Common.Interfaces.Persistence;
using ApiApplication.Domain.Common.DomainErrors;
using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ErrorOr<Reservation>>
{
    private readonly IReservationRepository _dinnerRepository;
    private readonly IShowTimeRepository _menuRepository;

    public CreateReservationCommandHandler(IReservationRepository dinnerRepository, IShowTimeRepository menuRepository)
    {
        _dinnerRepository = dinnerRepository;
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Reservation>> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var createMenuIdResult = ShowTimeId.Create(command.MenuId);

        if (createMenuIdResult.IsError)
        {
            return createMenuIdResult.Errors;
        }

        if (!await _menuRepository.ExistsAsync(createMenuIdResult.Value))
        {
            return Errors.ShowTime.NotFound;
        }

        //var dinner = Reservation.Create(
        //    command.Name,
        //    command.Description,
        //    command.StartDateTime,
        //    command.EndDateTime,
        //    command.IsPublic,
        //    command.MaxGuests,
        //    Price.Create(
        //        command.Price.Amount,
        //        command.Price.Currency),
        //    createMenuIdResult.Value,
        //    HostId.Create(command.HostId),
        //    command.ImageUrl,
        //    Location.Create(
        //        command.Location.Name,
        //        command.Location.Address,
        //        command.Location.Latitude,
        //        command.Location.Longitude));

        //await _dinnerRepository.AddAsync(dinner);

        return null;
    }
}