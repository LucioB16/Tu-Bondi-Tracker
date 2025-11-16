namespace TuBondi.Domain.Entities;

/// <summary>
/// Representa una alerta emitida por la API para alguna línea o la parada completa.
/// </summary>
public sealed record TransitNotification(string Message, string? Line = null);
