using System.Text.Json.Serialization;
using TuBondi.ApiClient.Serialization;

namespace TuBondi.ApiClient.Models;

/// <summary>
/// Representa el resultado de la consulta de próximos arribos por parada.
/// </summary>
public sealed class ArrivalsResponse
{
    /// <summary>
    /// Información básica de la parada consultada.
    /// </summary>
    [JsonPropertyName("parada")]
    public ParadaInfo? Parada { get; init; }

    /// <summary>
    /// Colección de arribos próximos (uno por línea o coche).
    /// </summary>
    [JsonPropertyName("proximos_arribos")]
    public IReadOnlyList<Arribo> ProximosArribos { get; init; } = Array.Empty<Arribo>();

    /// <summary>
    /// Preferencias de filtrado OnlyGPS devueltas por el backend.
    /// </summary>
    [JsonPropertyName("onlygps_array")]
    public IDictionary<string, bool> OnlyGpsArray { get; init; } = new Dictionary<string, bool>();
}

/// <summary>
/// Contiene los datos básicos de la parada consultada.
/// </summary>
public sealed class ParadaInfo
{
    /// <summary>
    /// Código público de la parada.
    /// </summary>
    [JsonPropertyName("codigo")]
    public string? Codigo { get; init; }

    /// <summary>
    /// Nombre descriptivo de la parada.
    /// </summary>
    [JsonPropertyName("nombre")]
    public string? Nombre { get; init; }

    /// <summary>
    /// Identificador de la línea principal asociada.
    /// </summary>
    [JsonPropertyName("linea")]
    public string? Linea { get; init; }

    /// <summary>
    /// Sentido en el que opera la parada.
    /// </summary>
    [JsonPropertyName("sentido")]
    public string? Sentido { get; init; }

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
}

/// <summary>
/// Describe un próximo arribo retornado por la API.
/// </summary>
public sealed class Arribo
{
    /// <summary>
    /// Línea asociada al arribo.
    /// </summary>
    [JsonPropertyName("linea")]
    public string? Linea { get; init; }

    /// <summary>
    /// Ruta específica del arribo.
    /// </summary>
    [JsonPropertyName("ruta")]
    public string? Ruta { get; init; }

    /// <summary>
    /// Texto descriptivo del próximo arribo (por ejemplo hora estimada).
    /// </summary>
    [JsonPropertyName("proximo")]
    public string? Proximo { get; init; }

    /// <summary>
    /// Texto alternativo de demora.
    /// </summary>
    [JsonPropertyName("demora")]
    public string? Demora { get; init; }

    /// <summary>
    /// Demora en minutos, si está disponible numéricamente.
    /// </summary>
    [JsonPropertyName("demora_minutos")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double DemoraMinutos { get; init; }

    /// <summary>
    /// Arreglo de valores auxiliares (<c>a</c>) reportados por la API.
    /// </summary>
    [JsonPropertyName("a")]
    public double[] A { get; init; } = Array.Empty<double>();
}
