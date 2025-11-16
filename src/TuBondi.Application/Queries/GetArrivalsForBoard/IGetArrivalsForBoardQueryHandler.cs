namespace TuBondi.Application.Queries.GetArrivalsForBoard;

/// <summary>
/// Contrato para procesar la consulta de arribos normalizados.
/// </summary>
public interface IGetArrivalsForBoardQueryHandler
{
    Task<GetArrivalsForBoardResult> HandleAsync(GetArrivalsForBoardQuery query, CancellationToken ct = default);
}
