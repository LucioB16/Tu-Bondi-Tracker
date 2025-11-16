using TuBondi.Domain.Entities;

namespace TuBondi.Application.Abstractions;

/// <summary>
/// Puerto que abstrae el origen de datos de TuBondi.
/// </summary>
public interface ITransitDataSource
{
    /// <summary>
    /// Inicializa cualquier estado requerido para operar contra la API.
    /// </summary>
    Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtiene los arribos crudos de una parada específica.
    /// </summary>
    Task<BoardSnapshot> GetArrivalsAsync(string stopCode, CancellationToken ct = default);
}
