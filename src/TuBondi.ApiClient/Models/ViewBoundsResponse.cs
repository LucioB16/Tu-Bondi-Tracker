using System.Text.Json.Serialization;

namespace TuBondi.ApiClient.Models;

/// <summary>
/// Describe los límites geográficos que el mapa debe mostrar.
/// </summary>
public sealed class ViewBoundsResponse
{
    /// <summary>
    /// Latitud máxima del recuadro en grados decimales.
    /// </summary>
    [JsonPropertyName("max_lat")]
    public double MaxLat { get; init; }

    /// <summary>
    /// Longitud máxima del recuadro en grados decimales.
    /// </summary>
    [JsonPropertyName("max_lon")]
    public double MaxLon { get; init; }

    /// <summary>
    /// Latitud mínima del recuadro en grados decimales.
    /// </summary>
    [JsonPropertyName("min_lat")]
    public double MinLat { get; init; }

    /// <summary>
    /// Longitud mínima del recuadro en grados decimales.
    /// </summary>
    [JsonPropertyName("min_lon")]
    public double MinLon { get; init; }
}
