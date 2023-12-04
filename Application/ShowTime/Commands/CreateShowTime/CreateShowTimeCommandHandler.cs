using ApiApplication.Domain.ShowTimeAggregate;
using Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Menus.Queries.ListMenus;
using ErrorOr;
using MediatR;

namespace ABuberDinner.Application.Menus.Queries.ListMenus;

public class CreateShowTimeCommandHandler : IRequestHandler<CreateShowTimeCommand, ErrorOr<ShowTime>>
{
    private readonly IShowTimeRepository _menuRepository;

    public CreateShowTimeCommandHandler(IShowTimeRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<ShowTime>> Handle(CreateShowTimeCommand command, CancellationToken cancellationToken)
    {
        var menu = ShowTime.Create(
            MovieId: MovieId.Create(command.MovieId),
            sessionDate: command.sessionDate,
            auditoriumId: command.auditoriumId,
            //sections: command.Sections.ConvertAll(section => MenuSection.Create(
            //    section.Name,
            //    section.Description,
            //    section.Items.ConvertAll(item => MenuItem.Create(
            //        item.Name,
            //        item.Description))))
                    );

        await _menuRepository.AddAsync(menu);

        return menu;
    }
}