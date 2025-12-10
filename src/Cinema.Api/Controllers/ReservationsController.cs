using Cinema.Application.Reservations.Commands.ConfirmReservation;
using Cinema.Application.Reservations.Commands.CreateReservation;
using Cinema.Contracts.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ISender _sender;

    public ReservationsController(ISender sender)
    {
        _sender = sender;
    }

    
    
    
    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            request.ShowtimeId,
            request.Seats.Select(s => new Application.Reservations.Commands.CreateReservation.SeatRequest(s.Row, s.Number)).ToList());

        var reservation = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetReservation),
            new { id = reservation.Id.Value },
            new ReservationResponse(
                reservation.Id.Value,
                reservation.ShowtimeId.Value,
                reservation.Seats.Select(s => new SeatResponse(s.Row, s.Number)).ToList(),
                reservation.Status.Value.ToString(),
                reservation.CreatedAt,
                reservation.ExpiresAt,
                reservation.ConfirmedAt));
    }

    
    
    
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmReservation(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmReservationCommand(id);
        var reservation = await _sender.Send(command, cancellationToken);

        return Ok(new ReservationResponse(
            reservation.Id.Value,
            reservation.ShowtimeId.Value,
            reservation.Seats.Select(s => new SeatResponse(s.Row, s.Number)).ToList(),
            reservation.Status.Value.ToString(),
            reservation.CreatedAt,
            reservation.ExpiresAt,
            reservation.ConfirmedAt));
    }

    
    
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservation(
        Guid id,
        CancellationToken cancellationToken)
    {
        
        return NotFound();
    }
}
