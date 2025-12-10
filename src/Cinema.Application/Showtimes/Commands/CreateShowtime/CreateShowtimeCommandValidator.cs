using FluentValidation;

namespace Cinema.Application.Showtimes.Commands.CreateShowtime;




public class CreateShowtimeCommandValidator : AbstractValidator<CreateShowtimeCommand>
{
    public CreateShowtimeCommandValidator()
    {
        RuleFor(x => x.MovieImdbId)
            .NotEmpty().WithMessage("Movie IMDb ID is required")
            .Matches(@"^tt\d{7,8}$").WithMessage("Invalid IMDb ID format");

        RuleFor(x => x.ScreeningTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Screening time must be in the future");

        RuleFor(x => x.AuditoriumId)
            .NotEmpty().WithMessage("Auditorium ID is required");
    }
}
