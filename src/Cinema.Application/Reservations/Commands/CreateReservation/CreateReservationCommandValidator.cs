using FluentValidation;

namespace Cinema.Application.Reservations.Commands.CreateReservation;




public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.ShowtimeId)
            .NotEmpty().WithMessage("Showtime ID is required");

        RuleFor(x => x.Seats)
            .NotEmpty().WithMessage("At least one seat must be selected")
            .Must(HaveValidSeats).WithMessage("All seats must have valid row and number");

        RuleFor(x => x.Seats)
            .Must(BeInSameRow).WithMessage("All seats must be in the same row")
            .When(x => x.Seats != null && x.Seats.Count > 1);

        RuleFor(x => x.Seats)
            .Must(BeContiguous).WithMessage("Seats must be contiguous (consecutive)")
            .When(x => x.Seats != null && x.Seats.Count > 1);
    }

    private bool HaveValidSeats(List<SeatRequest> seats)
    {
        return seats.All(s => s.Row > 0 && s.Number > 0);
    }

    private bool BeInSameRow(List<SeatRequest> seats)
    {
        return seats.Select(s => s.Row).Distinct().Count() == 1;
    }

    private bool BeContiguous(List<SeatRequest> seats)
    {
        var sortedSeats = seats.OrderBy(s => s.Number).ToList();
        for (int i = 0; i < sortedSeats.Count - 1; i++)
        {
            if (sortedSeats[i + 1].Number != sortedSeats[i].Number + 1)
                return false;
        }
        return true;
    }
}
