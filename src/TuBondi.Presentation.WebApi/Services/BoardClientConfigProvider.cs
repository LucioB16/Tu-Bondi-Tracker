using Microsoft.Extensions.Options;
using TuBondi.Presentation.WebApi.Options;

namespace TuBondi.Presentation.WebApi.Services;

/// <summary>
/// Expone la configuración del tablero al cliente web.
/// </summary>
public interface IBoardClientConfigProvider
{
    BoardClientConfig GetConfig();
}

public sealed class BoardClientConfig
{
    public required string StopCode { get; init; }
    public required string StopDescription { get; init; }
    public required IReadOnlyList<string> Lines { get; init; }
    public required TimeSpan UpdateInterval { get; init; }
}

public sealed class BoardClientConfigProvider : IBoardClientConfigProvider
{
    private readonly TrackerOptions _options;

    public BoardClientConfigProvider(IOptions<TrackerOptions> options)
    {
        _options = options.Value;
    }

    public BoardClientConfig GetConfig()
        => new()
        {
            StopCode = _options.StopCode,
            StopDescription = _options.StopDescription,
            Lines = _options.GetLines(),
            UpdateInterval = TimeSpan.FromSeconds(Math.Clamp(_options.UpdateIntervalSeconds, 5, 120))
        };
}
