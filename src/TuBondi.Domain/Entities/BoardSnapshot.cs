namespace TuBondi.Domain.Entities;

/// <summary>
/// Representa el estado completo del tablero para un instante dado.
/// </summary>
public sealed record BoardSnapshot(
    StopSnapshot Stop,
    IReadOnlyList<TransitArrival> Arrivals,
    IReadOnlyList<TransitNotification> Notifications,
    DateTimeOffset GeneratedAt,
    string? ErrorMessage);
