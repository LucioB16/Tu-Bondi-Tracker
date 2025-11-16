using System.Text.Json.Serialization;
using TuBondi.ApiClient.Serialization;

namespace TuBondi.ApiClient.Models;

/// <summary>
/// Describe la respuesta al seleccionar la traza de una ruta.
/// </summary>
public sealed class RouteSelectionResponse
{
    /// <summary>
    /// Colección de puntos que describen la geometría de la traza.
    /// </summary>
    [JsonPropertyName("traza")]
    public IReadOnlyList<TrazaPoint> Traza { get; init; } = Array.Empty<TrazaPoint>();

    /// <summary>
    /// Paradas asociadas a la ruta seleccionada.
    /// </summary>
    [JsonPropertyName("paradas")]
    public IReadOnlyList<Parada> Paradas { get; init; } = Array.Empty<Parada>();

    /// <summary>
    /// Color sugerido para la traza.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Sentido textual de la traza (por ejemplo, ida o vuelta).
    /// </summary>
    [JsonPropertyName("sentido")]
    public string? Sentido { get; init; }

    /// <summary>
    /// Mensaje informativo opcional proporcionado por el backend.
    /// </summary>
    [JsonPropertyName("notificacion")]
    public string? Notificacion { get; init; }

    /// <summary>
    /// Lista detallada de notificaciones estructuradas.
    /// </summary>
    [JsonPropertyName("notificaciones")]
    public IReadOnlyList<Notificacion> Notificaciones { get; init; } = Array.Empty<Notificacion>();
}

/// <summary>
/// Representa un punto individual de la traza en formato lon/lat/curso.
/// </summary>
/// <param name="Lon">Longitud en grados decimales.</param>
/// <param name="Lat">Latitud en grados decimales.</param>
/// <param name="Course">Curso o rumbo expresado en grados.</param>
[JsonConverter(typeof(TrazaPointConverter))]
public sealed record TrazaPoint(double Lon, double Lat, double Course);

/// <summary>
/// Describe una parada perteneciente a una ruta.
/// </summary>
public sealed class Parada
{
    /// <summary>
    /// Código interno de la parada.
    /// </summary>
    [JsonPropertyName("codigo")]
    public string? Codigo { get; init; }

    /// <summary>
    /// Línea que atiende la parada.
    /// </summary>
    [JsonPropertyName("linea")]
    public string? Linea { get; init; }

    /// <summary>
    /// Nombre descriptivo de la parada.
    /// </summary>
    [JsonPropertyName("nombre")]
    public string? Nombre { get; init; }

    /// <summary>
    /// Color sugerido para iconografía.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Latitud de la parada en grados decimales.
    /// </summary>
    [JsonPropertyName("lat")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Lat { get; init; }

    /// <summary>
    /// Longitud de la parada en grados decimales.
    /// </summary>
    [JsonPropertyName("lon")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Lon { get; init; }

    /// <summary>
    /// Nombre alternativo mostrado al usuario.
    /// </summary>
    [JsonPropertyName("parada_nombre")]
    public string? ParadaNombre { get; init; }

    /// <summary>
    /// Curso o ángulo del itinerario en la parada.
    /// </summary>
    [JsonPropertyName("curso")]
    public string? Curso { get; init; }

    /// <summary>
    /// Identificador textual del itinerario.
    /// </summary>
    [JsonPropertyName("itinerario")]
    public string? Itinerario { get; init; }

    /// <summary>
    /// Identificador del cliente asociado.
    /// </summary>
    [JsonPropertyName("cliente")]
    public string? Cliente { get; init; }

    /// <summary>
    /// Sentido en el que opera la parada.
    /// </summary>
    [JsonPropertyName("sentido")]
    public string? Sentido { get; init; }
}

/// <summary>
/// Representa una notificación estructurada de la API.
/// </summary>
public sealed class Notificacion
{
    /// <summary>
    /// Texto descriptivo de la notificación.
    /// </summary>
    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; init; }

    /// <summary>
    /// Tipo o severidad de la notificación.
    /// </summary>
    [JsonPropertyName("tipo")]
    public string? Tipo { get; init; }
}
