using TuBondi.Domain.Entities;

namespace TuBondi.Application.Queries.GetArrivalsForBoard;

/// <summary>
/// Resultado del procesamiento de arribos.
/// </summary>
public sealed record GetArrivalsForBoardResult(
    StopSnapshot Stop,
    IReadOnlyList<TransitArrival> Arrivals,
    IReadOnlyList<TransitNotification> Notifications,
    DateTimeOffset GeneratedAt,
    string? ErrorMessage);
