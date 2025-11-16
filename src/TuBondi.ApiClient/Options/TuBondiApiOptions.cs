namespace TuBondi.ApiClient.Options;

/// <summary>
/// Define valores predeterminados y ajustes de red para comunicarse con la API TuBondi.
/// </summary>
/// <remarks>
/// Todas las propiedades incluyen valores sugeridos que replican el comportamiento esperado por los endpoints de TuBondi.
/// </remarks>
public sealed class TuBondiApiOptions
{
    /// <summary>
    /// Obtiene o establece la dirección base desde donde se resuelven todos los endpoints.
    /// </summary>
    /// <remarks>
    /// El valor predeterminado apunta al host público provisto por el municipio de Córdoba.
    /// </remarks>
    public Uri BaseAddress { get; set; } = new("https://micronauta4.dnsalias.net/");

    /// <summary>
    /// Obtiene o establece la configuración de ciudad utilizada en los formularios enviados a la API.
    /// </summary>
    /// <remarks>
    /// Generalmente corresponde al identificador de ciudad disponible en el panel web, por ejemplo <c>cbaciudad</c>.
    /// </remarks>
    public string Conf { get; set; } = "cbaciudad";

    /// <summary>
    /// Obtiene o establece el tiempo máximo permitido para cada operación HTTP.
    /// </summary>
    /// <remarks>
    /// El valor predeterminado de diez segundos ofrece un balance entre responsividad y tolerancia ante picos de red.
    /// </remarks>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Obtiene o establece el encabezado <c>User-Agent</c> que se enviará en cada solicitud.
    /// </summary>
    /// <remarks>
    /// Use esta propiedad para identificar la integración respetando las políticas del servicio.
    /// </remarks>
    public string UserAgent { get; set; } = "TuBondi.ApiClient/1.0 (+https://example.local)";
}
