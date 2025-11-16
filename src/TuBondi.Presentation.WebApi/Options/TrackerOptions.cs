namespace TuBondi.Presentation.WebApi.Options;

/// <summary>
/// Configuración del tablero web.
/// </summary>
public sealed class TrackerOptions
{
    public string StopCode { get; set; } = "A911";
    public string StopDescription { get; set; } = "AV. DUARTE QUIROS 1717-1728";
    public string Lines { get; set; } = "40,42,71";
    public int UpdateIntervalSeconds { get; set; } = 25;

    public IReadOnlyList<string> GetLines()
        => Lines.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
