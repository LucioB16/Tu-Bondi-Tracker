namespace TuBondi.Domain.Entities;

/// <summary>
/// Describe la parada y texto asociado a la consulta actual.
/// </summary>
public sealed record StopSnapshot(string Code, string Description);
