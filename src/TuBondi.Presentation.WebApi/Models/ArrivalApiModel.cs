namespace TuBondi.Presentation.WebApi.Models;

/// <summary>
/// Representa el payload enviado al frontend para cada arribo.
/// </summary>
public sealed record ArrivalApiModel(
    string Line,
    int? EtaMinutes,
    double? DistanceKm,
    string Route,
    string Direction,
    string Operator,
    string Color,
    string? Notification,
    string? RawMessage);
