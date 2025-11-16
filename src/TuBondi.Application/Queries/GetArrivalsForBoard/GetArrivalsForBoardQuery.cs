namespace TuBondi.Application.Queries.GetArrivalsForBoard;

/// <summary>
/// Solicitud de arribos para un tablero determinado.
/// </summary>
public sealed record GetArrivalsForBoardQuery(
    string StopCode,
    IReadOnlyCollection<string> Lines,
    int MaxArrivals = 6);
