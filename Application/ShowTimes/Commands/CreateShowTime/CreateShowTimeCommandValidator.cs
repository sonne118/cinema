using ApiApplication.Application.ShowTimes.Command.CreateShowTime;
using FluentValidation;

namespace ApiApplication.Application.ShowTimes.Command.CreateShowTime;

public class CreateShowTimeCommandValidator : AbstractValidator<CreateShowTimeCommand>
{
    public CreateShowTimeCommandValidator()
    {
        RuleFor(x => x.auditoriumId).NotEmpty();
        RuleFor(x => x.sessionDate).NotEmpty();
        RuleFor(x => x.MovieId).NotEmpty();
    }
}