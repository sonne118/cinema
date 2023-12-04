using BuberDinner.Application.Menus.Queries.ListMenus;
using FluentValidation;

namespace BuberDinner.Application.Menus.Commands.CreateMenu;

public class CreateShowTimeCommandValidator : AbstractValidator<CreateShowTimeCommand>
{
    public CreateShowTimeCommandValidator()
    {
        RuleFor(x => x.auditoriumId).NotEmpty();
        RuleFor(x => x.sessionDate).NotEmpty();
        RuleFor(x => x.movieId).NotEmpty();
    }
}