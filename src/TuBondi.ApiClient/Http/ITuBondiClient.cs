using TuBondi.ApiClient.Models;

namespace TuBondi.ApiClient.Http;

/// <summary>
/// Contrato que expone las operaciones disponibles en la API TuBondi.
/// </summary>
public interface ITuBondiClient
{
    /// <summary>
    /// Inicializa la sesión solicitando el HTML público para obtener la cookie <c>PHPSESSID</c>.
    /// </summary>
    /// <param name="ct">Token de cancelación cooperativa.</param>
    /// <returns>Tarea completada cuando la cookie se haya capturado.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si la API no entrega la cookie esperada.</exception>
    Task InitializeSessionAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtiene las líneas y rutas disponibles para la configuración indicada.
    /// </summary>
    /// <param name="conf">Configuración de ciudad alternativa.</param>
    /// <param name="ct">Token de cancelación cooperativa.</param>
    /// <returns>Lista de líneas, rutas, grupos y clientes.</returns>
    Task<LinesRoutesResponse> GetLinesAndRoutesAsync(string? conf = null, CancellationToken ct = default);

    /// <summary>
    /// Obtiene los límites geográficos de la vista.
    /// </summary>
    /// <param name="conf">Configuración de ciudad alternativa.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Límites máximos y mínimos de latitud y longitud.</returns>
    Task<ViewBoundsResponse> GetViewBoundsAsync(string? conf = null, CancellationToken ct = default);

    /// <summary>
    /// Selecciona la traza de una ruta determinada.
    /// </summary>
    /// <param name="rutaId">Identificador de la ruta.</param>
    /// <param name="clienteId">Identificador del cliente.</param>
    /// <param name="conf">Configuración alternativa.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Traza, paradas y notificaciones asociadas.</returns>
    Task<RouteSelectionResponse> SelectRouteTraceAsync(int rutaId, int clienteId, string? conf = null, CancellationToken ct = default);

    /// <summary>
    /// Consulta los coches rastreados para una ruta.
    /// </summary>
    /// <param name="rutaId">Identificador de la ruta.</param>
    /// <param name="clienteId">Identificador del cliente.</param>
    /// <param name="stopCode">Código de parada opcional para filtrar.</param>
    /// <param name="conf">Configuración alternativa.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Listado de coches con su última posición.</returns>
    Task<VehiclesByRouteResponse> QueryVehiclesByRouteAsync(int rutaId, int clienteId, string? stopCode = null, string? conf = null, CancellationToken ct = default);

    /// <summary>
    /// Obtiene los próximos arribos para una parada.
    /// </summary>
    /// <param name="stopCode">Código de parada.</param>
    /// <param name="conf">Configuración alternativa.</param>
    /// <param name="show80min">Indica si se deben mostrar arribos lejanos.</param>
    /// <param name="onlyGps">Filtro opcional OnlyGPS.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Arribos y metadatos de la parada.</returns>
    Task<ArrivalsResponse> GetArrivalsAsync(string stopCode, string? conf = null, bool show80min = false, IDictionary<int, bool>? onlyGps = null, CancellationToken ct = default);

    /// <summary>
    /// Obtiene el valor actual de la cookie <c>PHPSESSID</c> capturada.
    /// </summary>
    string? CurrentPhpSessionId { get; }
}
