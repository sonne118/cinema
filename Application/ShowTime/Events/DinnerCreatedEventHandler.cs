using ApiApplication.Domain.TicketAggregate;
using Application.Common.Interfaces.Persistence;
using MediatR;

namespace BuberDinner.Application.Menus.Events;

public class DinnerCreatedEventHandler : INotificationHandler<ReservationCreated>
{
    private readonly IShowTimeRepository _menuRepository;

    public DinnerCreatedEventHandler(IShowTimeRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task Handle(ReservationCreated reservationCreatedEvent, CancellationToken cancellationToken)
    {
        //if (await _menuRepository.GetByIdAsync(dinnerCreatedEvent.Dinner.MenuId) is not Menu menu)
        //{
        //    throw new InvalidOperationException($"Dinner has invalid menu id (dinner id: {dinnerCreatedEvent.Dinner.Id}, menu id: {dinnerCreatedEvent.Dinner.MenuId}).");
        //}

        //menu.AddDinnerId((DinnerId)dinnerCreatedEvent.Dinner.Id);

        //await _menuRepository.UpdateAsync(menu);
        return null;
    }
}