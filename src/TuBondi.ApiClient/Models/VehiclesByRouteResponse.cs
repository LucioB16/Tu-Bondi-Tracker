using System.Text.Json.Serialization;
using TuBondi.ApiClient.Serialization;

namespace TuBondi.ApiClient.Models;

/// <summary>
/// Describe la respuesta del comando <c>consultacocheporruta</c>.
/// </summary>
public sealed class VehiclesByRouteResponse
{
    /// <summary>
    /// Colección de coches rastreados en la ruta solicitada.
    /// </summary>
    [JsonPropertyName("coches")]
    public IReadOnlyList<Coche> Coches { get; init; } = Array.Empty<Coche>();

    /// <summary>
    /// Notificación de texto libre asociada a la consulta.
    /// </summary>
    [JsonPropertyName("notificacion")]
    public string? Notificacion { get; init; }

    /// <summary>
    /// Detalles adicionales de error proporcionados por el backend.
    /// </summary>
    [JsonPropertyName("error")]
    public object? Error { get; init; }
}

/// <summary>
/// Describe el estado de un coche en tiempo real.
/// </summary>
public sealed class Coche
{
    /// <summary>
    /// Identificador del coche.
    /// </summary>
    [JsonPropertyName("coche")]
    public string? CocheId { get; init; }

    /// <summary>
    /// Línea a la que pertenece el coche.
    /// </summary>
    [JsonPropertyName("linea")]
    public string? Linea { get; init; }

    /// <summary>
    /// Ruta en la que circula actualmente.
    /// </summary>
    [JsonPropertyName("ruta")]
    public string? Ruta { get; init; }

    /// <summary>
    /// Cliente u operador asociado.
    /// </summary>
    [JsonPropertyName("cliente")]
    public string? Cliente { get; init; }

    /// <summary>
    /// Demora informada en minutos. El valor <c>99999</c> indica información no disponible.
    /// </summary>
    [JsonPropertyName("demora_minutos")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double DemoraMinutos { get; init; }

    /// <summary>
    /// Velocidad aproximada en kilómetros por hora.
    /// </summary>
    [JsonPropertyName("velocidad")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Velocidad { get; init; }

    /// <summary>
    /// Marca de tiempo en formato de texto devuelta por el backend.
    /// </summary>
    [JsonPropertyName("tiempo")]
    public string? Tiempo { get; init; }

    /// <summary>
    /// Coordenada de latitud del vehículo.
    /// </summary>
    [JsonPropertyName("lat")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Lat { get; init; }

    /// <summary>
    /// Coordenada de longitud del vehículo.
    /// </summary>
    [JsonPropertyName("lon")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Lon { get; init; }

    /// <summary>
    /// Valor entero auxiliar reportado como <c>i</c> por la API (p.ej. prioridad o índice).
    /// </summary>
    [JsonPropertyName("i")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int I { get; init; }

    /// <summary>
    /// Curso reportado en grados.
    /// </summary>
    [JsonPropertyName("curso")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Curso { get; init; }
}
