namespace Cinema.Contracts.Reservations;

public record CreateReservationRequest(
    Guid ShowtimeId,
    List<SeatRequest> Seats);

public record SeatRequest(int Row, int Number);

public record ConfirmReservationRequest(Guid ReservationId);

public record ReservationResponse(
    Guid Id,
    Guid ShowtimeId,
    List<SeatResponse> Seats,
    string Status,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? ConfirmedAt);

public record SeatResponse(int Row, int Number);
