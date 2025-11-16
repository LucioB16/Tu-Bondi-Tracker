using Microsoft.Extensions.Logging;
using TuBondi.Application.Abstractions;
using TuBondi.Domain.Entities;

namespace TuBondi.Application.Queries.GetArrivalsForBoard;

/// <summary>
/// Implementación predeterminada del caso de uso que alimenta el tablero LED.
/// </summary>
public sealed class GetArrivalsForBoardQueryHandler : IGetArrivalsForBoardQueryHandler
{
    private readonly ITransitDataSource _dataSource;
    private readonly ILogger<GetArrivalsForBoardQueryHandler> _logger;

    public GetArrivalsForBoardQueryHandler(ITransitDataSource dataSource, ILogger<GetArrivalsForBoardQueryHandler> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<GetArrivalsForBoardResult> HandleAsync(GetArrivalsForBoardQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var normalizedLines = query.Lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim())
            .Select(l => l.ToUpperInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _logger.LogInformation("Fetching TuBondi arrivals for stop {Stop} with {Count} lines", query.StopCode, normalizedLines.Count);

        var rawSnapshot = await _dataSource.GetArrivalsAsync(query.StopCode, ct);
        var filteredArrivals = rawSnapshot.Arrivals
            .Where(a => normalizedLines.Count == 0 || normalizedLines.Contains(a.Line.ToUpperInvariant()))
            .OrderBy(a => a.EtaMinutes ?? int.MaxValue)
            .ThenBy(a => a.Line)
            .Take(query.MaxArrivals)
            .ToList();

        var filteredNotifications = rawSnapshot.Notifications
            .Where(n => n.Line is null || normalizedLines.Contains(n.Line.ToUpperInvariant()))
            .ToList();

        var snapshot = new BoardSnapshot(
            rawSnapshot.Stop,
            filteredArrivals,
            filteredNotifications,
            rawSnapshot.GeneratedAt,
            rawSnapshot.ErrorMessage);

        return new GetArrivalsForBoardResult(
            snapshot.Stop,
            snapshot.Arrivals,
            snapshot.Notifications,
            snapshot.GeneratedAt,
            snapshot.ErrorMessage);
    }
}
