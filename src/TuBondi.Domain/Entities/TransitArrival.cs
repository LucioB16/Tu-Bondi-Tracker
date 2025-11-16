namespace TuBondi.Domain.Entities;

/// <summary>
/// Representa un arribo normalizado listo para mostrarse en el tablero.
/// </summary>
/// <remarks>
/// Todos los comentarios se mantienen en español siguiendo las pautas del proyecto.
/// </remarks>
public sealed record TransitArrival(
    string Line,
    string Route,
    string Direction,
    string Operator,
    double? DistanceKm,
    int? EtaMinutes,
    string ColorHex,
    string? RawMessage,
    string? Notification)
{
    /// <summary>
    /// Indica si el arribo está próximo (cinco minutos o menos).
    /// </summary>
    public bool IsApproaching => EtaMinutes is not null and <= 5;
}
