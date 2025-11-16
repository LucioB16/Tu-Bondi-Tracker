namespace TuBondi.Infrastructure.Options;

/// <summary>
/// Configuración dedicada al adaptador que conversa con la API.
/// </summary>
public sealed class TuBondiGatewayOptions
{
    public string Conf { get; set; } = "cbaciudad";
    public bool ShowExtendedHorizon { get; set; } = false;
    public double CacheDurationSeconds { get; set; } = 12;
}
