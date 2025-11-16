using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TuBondi.ApiClient.Models;
using TuBondi.ApiClient.Options;
using TuBondi.ApiClient.Serialization;

namespace TuBondi.ApiClient.Http;

/// <summary>
/// Implementación concreta de <see cref="ITuBondiClient"/> basada en <see cref="HttpClient"/>.
/// </summary>
public sealed class TuBondiClient : ITuBondiClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly TuBondiApiOptions _options;
    private readonly CookieContainer _cookieContainer;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private string? _currentSessionId;

    /// <summary>
    /// Crea una instancia utilizando un <see cref="SocketsHttpHandler"/> propio.
    /// </summary>
    /// <param name="options">Opciones de la API que definen base, tiempo de espera y agente.</param>
    public TuBondiClient(TuBondiApiOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _cookieContainer = new CookieContainer();
        var handler = new SocketsHttpHandler
        {
            CookieContainer = _cookieContainer,
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = true,
            UseCookies = true
        };
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = options.BaseAddress,
            Timeout = options.Timeout
        };
        _disposeHttpClient = true;
        ConfigureDefaults(_httpClient, options);
        _serializerOptions = CreateSerializerOptions();
    }

    /// <summary>
    /// Crea una instancia reutilizando un <see cref="HttpClient"/> externo.
    /// </summary>
    /// <param name="httpClient">Cliente HTTP previamente configurado.</param>
    /// <param name="options">Opciones a utilizar para encabezados y parámetros.</param>
    /// <param name="cookieContainer">Contenedor de cookies compartido para rastrear PHPSESSID.</param>
    public TuBondiClient(HttpClient httpClient, TuBondiApiOptions options, CookieContainer cookieContainer)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _cookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
        _httpClient.BaseAddress ??= options.BaseAddress;
        _httpClient.Timeout = options.Timeout;
        ConfigureDefaults(_httpClient, options);
        _serializerOptions = CreateSerializerOptions();
    }

    /// <inheritdoc />
    public string? CurrentPhpSessionId => _currentSessionId;

    /// <inheritdoc />
    public async Task InitializeSessionAsync(CancellationToken ct = default)
    {
        var confValue = Uri.EscapeDataString(_options.Conf);
        var target = $"{TuBondiApiRoutes.WebUrbano}?conf={confValue}";
        using var request = new HttpRequestMessage(HttpMethod.Get, target);
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        await EnsureSuccessAsync(response, ct).ConfigureAwait(false);
        _currentSessionId = ExtractSessionId();
        if (string.IsNullOrEmpty(_currentSessionId))
        {
            throw new InvalidOperationException("La API no devolvió la cookie PHPSESSID requerida para continuar");
        }
    }

    /// <inheritdoc />
    public Task<LinesRoutesResponse> GetLinesAndRoutesAsync(string? conf = null, CancellationToken ct = default)
    {
        var builder = new FormDataBuilder().Add("conf", conf ?? _options.Conf);
        return PostFormAsync<LinesRoutesResponse>(TuBondiApiRoutes.Cmd, "lineasyrutas", null, builder, ct);
    }

    /// <inheritdoc />
    public Task<ViewBoundsResponse> GetViewBoundsAsync(string? conf = null, CancellationToken ct = default)
    {
        var builder = new FormDataBuilder().Add("conf", conf ?? _options.Conf);
        return PostFormAsync<ViewBoundsResponse>(TuBondiApiRoutes.Cmd, "vista", null, builder, ct);
    }

    /// <inheritdoc />
    public Task<RouteSelectionResponse> SelectRouteTraceAsync(int rutaId, int clienteId, string? conf = null, CancellationToken ct = default)
    {
        var builder = new FormDataBuilder()
            .Add("ruta", rutaId)
            .Add("cliente_id", clienteId)
            .Add("conf", conf ?? _options.Conf);
        return PostFormAsync<RouteSelectionResponse>(TuBondiApiRoutes.Cmd, "seleccionatraza", null, builder, ct);
    }

    /// <inheritdoc />
    public Task<VehiclesByRouteResponse> QueryVehiclesByRouteAsync(int rutaId, int clienteId, string? stopCode = null, string? conf = null, CancellationToken ct = default)
    {
        var builder = new FormDataBuilder()
            .Add("ruta", rutaId)
            .Add("coche", 0)
            .Add("cliente", clienteId)
            .Add("conf", conf ?? _options.Conf);
        if (!string.IsNullOrWhiteSpace(stopCode))
        {
            builder.Add("parada_seleccionada", stopCode);
        }

        return PostFormAsync<VehiclesByRouteResponse>(TuBondiApiRoutes.Cmd, "consultacocheporruta", null, builder, ct);
    }

    /// <inheritdoc />
    public Task<ArrivalsResponse> GetArrivalsAsync(string stopCode, string? conf = null, bool show80min = false, IDictionary<int, bool>? onlyGps = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(stopCode))
        {
            throw new ArgumentException("El código de parada es obligatorio", nameof(stopCode));
        }

        var builder = new FormDataBuilder()
            .Add("conf", conf ?? _options.Conf)
            .Add("codigo", stopCode)
            .Add("show80min", show80min.ToString().ToLowerInvariant())
            .Add("onlygps_array", BuildOnlyGpsJson(onlyGps));

        return PostFormAsync<ArrivalsResponse>(TuBondiApiRoutes.Cmd, null, "proximos_arribos", builder, ct);
    }

    /// <summary>
    /// Libera los recursos gestionados del cliente HTTP.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    /// <summary>
    /// Configura encabezados por defecto y agente de usuario.
    /// </summary>
    /// <param name="client">Instancia de <see cref="HttpClient"/> a configurar.</param>
    /// <param name="options">Opciones desde donde se leen los valores.</param>
    private static void ConfigureDefaults(HttpClient client, TuBondiApiOptions options)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("es-419"));
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("es", 0.9));
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
    }

    /// <summary>
    /// Construye las opciones de serialización JSON para todos los modelos.
    /// </summary>
    /// <returns>Instancia configurada de <see cref="JsonSerializerOptions"/>.</returns>
    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        options.Converters.Add(new FlexibleDoubleConverter());
        options.Converters.Add(new FlexibleIntConverter());
        options.Converters.Add(new TrazaPointConverter());
        return options;
    }

    /// <summary>
    /// Ejecuta un POST con contenido form-url-encoded y deserializa la respuesta.
    /// </summary>
    /// <typeparam name="T">Tipo esperado en la respuesta.</typeparam>
    /// <param name="path">Ruta relativa a la base.</param>
    /// <param name="cmdQuery">Comando a enviar en la cadena de consulta.</param>
    /// <param name="cmdBody">Comando a enviar en el cuerpo del formulario.</param>
    /// <param name="builder">Instancia que contiene los parámetros adicionales.</param>
    /// <param name="ct">Token de cancelación cooperativa.</param>
    /// <returns>Instancia deserializada del tipo solicitado.</returns>
    private async Task<T> PostFormAsync<T>(string path, string? cmdQuery, string? cmdBody, FormDataBuilder builder, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(cmdBody))
        {
            builder.Add("cmd", cmdBody);
        }

        using var content = builder.Build();
        var targetPath = BuildPath(path, cmdQuery);
        using var request = new HttpRequestMessage(HttpMethod.Post, targetPath)
        {
            Content = content
        };
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        await EnsureSuccessAsync(response, ct).ConfigureAwait(false);
        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        var payload = await JsonSerializer.DeserializeAsync<T>(stream, _serializerOptions, ct).ConfigureAwait(false);
        if (payload is null)
        {
            throw new InvalidOperationException($"No fue posible deserializar la respuesta a {typeof(T).Name}");
        }

        return payload;
    }

    /// <summary>
    /// Construye la ruta combinando comando en query cuando corresponde.
    /// </summary>
    /// <param name="path">Ruta base.</param>
    /// <param name="cmdQuery">Comando opcional.</param>
    /// <returns>Cadena lista para invocar mediante <see cref="HttpClient"/>.</returns>
    private static string BuildPath(string path, string? cmdQuery)
    {
        if (string.IsNullOrEmpty(cmdQuery))
        {
            return path;
        }

        var separator = path.Contains('?') ? '&' : '?';
        return $"{path}{separator}cmd={Uri.EscapeDataString(cmdQuery)}";
    }

    /// <summary>
    /// Valida el código HTTP y arroja una excepción con contexto cuando falla.
    /// </summary>
    /// <param name="response">Respuesta devuelta por la API.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Tarea completada cuando la respuesta sea exitosa.</returns>
    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var snippetBuilder = new StringBuilder();
        if (response.Content != null)
        {
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            snippetBuilder.Append(body.Length > 512 ? body[..512] : body);
        }

        var request = response.RequestMessage;
        throw new HttpRequestException($"Error {(int)response.StatusCode} {response.ReasonPhrase} al invocar {request?.Method} {request?.RequestUri}. Resumen: {snippetBuilder}");
    }

    /// <summary>
    /// Serializa el diccionario OnlyGPS al formato requerido por el backend.
    /// </summary>
    /// <param name="onlyGps">Diccionario opcional con claves enteras.</param>
    /// <returns>Cadena JSON normalizada.</returns>
    private string BuildOnlyGpsJson(IDictionary<int, bool>? onlyGps)
    {
        IDictionary<string, bool> normalized;
        if (onlyGps is null || onlyGps.Count == 0)
        {
            normalized = new Dictionary<string, bool>();
        }
        else
        {
            normalized = onlyGps.ToDictionary(kvp => kvp.Key.ToString(CultureInfo.InvariantCulture), kvp => kvp.Value);
        }

        return JsonSerializer.Serialize(normalized, _serializerOptions);
    }

    /// <summary>
    /// Obtiene el valor actual de la cookie PHPSESSID almacenada en el contenedor.
    /// </summary>
    /// <returns>Valor textual de la cookie, si existe.</returns>
    private string? ExtractSessionId()
    {
        var baseUri = new Uri(_options.BaseAddress.GetLeftPart(UriPartial.Authority));
        var cookies = _cookieContainer.GetCookies(baseUri);
        return cookies["PHPSESSID"]?.Value;
    }
}
