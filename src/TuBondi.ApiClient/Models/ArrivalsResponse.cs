using System.Text.Json;
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
    [JsonConverter(typeof(OnlyGpsArrayConverter))]
    public IDictionary<string, bool> OnlyGpsArray { get; init; } = new Dictionary<string, bool>();

    /// <summary>
    /// Campo crudo devuelto bajo la clave <c>respuesta</c> utilizado como diagnóstico.
    /// </summary>
    [JsonPropertyName("respuesta")]
    public JsonElement? Respuesta { get; init; }

    /// <summary>
    /// Campo crudo devuelto bajo la clave <c>err</c> (a menudo <c>null</c> o texto).
    /// </summary>
    [JsonPropertyName("err")]
    public JsonElement? Err { get; init; }

    /// <summary>
    /// Campo crudo devuelto bajo la clave <c>x</c> cuyo significado varía según el backend.
    /// </summary>
    [JsonPropertyName("x")]
    public JsonElement? X { get; init; }
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
    /// Descripción amigable de la parada.
    /// </summary>
    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; init; }
}

/// <summary>
/// Describe un próximo arribo retornado por la API.
/// </summary>
public sealed class Arribo
{
    /// <summary>
    /// Identificador del coche reportado.
    /// </summary>
    [JsonPropertyName("coche")]
    public string? Coche { get; init; }

    /// <summary>
    /// Línea asociada al arribo.
    /// </summary>
    [JsonPropertyName("linea")]
    public string? Linea { get; init; }

    /// <summary>
    /// Nombre descriptivo de la línea reportada.
    /// </summary>
    [JsonPropertyName("linea_nombre")]
    public string? LineaNombre { get; init; }

    /// <summary>
    /// Ruta específica del arribo.
    /// </summary>
    [JsonPropertyName("ruta")]
    public string? Ruta { get; init; }

    /// <summary>
    /// Descripción de la ruta programada.
    /// </summary>
    [JsonPropertyName("ruta_descripcion")]
    public string? RutaDescripcion { get; init; }

    /// <summary>
    /// Código de la próxima ruta según programación.
    /// </summary>
    [JsonPropertyName("ruta_siguiente")]
    public string? RutaSiguiente { get; init; }

    /// <summary>
    /// Sentido operativo del arribo.
    /// </summary>
    [JsonPropertyName("sentido")]
    public string? Sentido { get; init; }

    /// <summary>
    /// Indicador de pantalla configurado para el servicio.
    /// </summary>
    [JsonPropertyName("pantalla")]
    public string? Pantalla { get; init; }

    /// <summary>
    /// Indicador de rampa de accesibilidad.
    /// </summary>
    [JsonPropertyName("rampa")]
    public string? Rampa { get; init; }

    /// <summary>
    /// Número de serie del vehículo.
    /// </summary>
    [JsonPropertyName("serie")]
    public string? Serie { get; init; }

    /// <summary>
    /// Sentido programado en el horario.
    /// </summary>
    [JsonPropertyName("horario_sentido")]
    public string? HorarioSentido { get; init; }

    /// <summary>
    /// Media vuelta programada en el horario.
    /// </summary>
    [JsonPropertyName("horario_media_vuelta")]
    public string? HorarioMediaVuelta { get; init; }

    /// <summary>
    /// Color sugerido del servicio.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Comentario libre asociado al arribo.
    /// </summary>
    [JsonPropertyName("comentario")]
    public string? Comentario { get; init; }

    /// <summary>
    /// Texto descriptivo del próximo arribo (por ejemplo hora estimada).
    /// </summary>
    [JsonPropertyName("proximo")]
    public string? Proximo { get; init; }

    /// <summary>
    /// Texto alternativo de demora (por ejemplo <c>"49min"</c>).
    /// </summary>
    [JsonPropertyName("demora")]
    public string? Demora { get; init; }

    /// <summary>
    /// Arreglo posicional de cuatro valores dobles (<c>lon1</c>, <c>lat1</c>, <c>lon2</c>, <c>lat2</c>).
    /// </summary>
    [JsonPropertyName("a")]
    public double[] A { get; init; } = Array.Empty<double>();

    /// <summary>
    /// Distancia en metros hasta la parada, reportada como entero.
    /// </summary>
    [JsonPropertyName("dist_parada")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int DistParada { get; init; }

    /// <summary>
    /// Distancia restante en kilómetros, reportada como número o cadena.
    /// </summary>
    [JsonPropertyName("distancia")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Distancia { get; init; }

    /// <summary>
    /// Hora teórica ajustada en formato <c>HH:mm:ss</c>, cuando está disponible.
    /// </summary>
    [JsonPropertyName("horaTeoricaAjustada")]
    public string? HoraTeoricaAjustada { get; init; }

    /// <summary>
    /// Hora teórica original proporcionada por el backend.
    /// </summary>
    [JsonPropertyName("horaTeorica")]
    public string? HoraTeorica { get; init; }

    /// <summary>
    /// Hora de salida prevista para el servicio.
    /// </summary>
    [JsonPropertyName("hora_salida")]
    public string? HoraSalida { get; init; }

    /// <summary>
    /// Cliente asociado al arribo.
    /// </summary>
    [JsonPropertyName("cliente")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Cliente { get; init; }

    /// <summary>
    /// Nombre del cliente según catálogo.
    /// </summary>
    [JsonPropertyName("cliente_nombre")]
    public string? ClienteNombre { get; init; }

    /// <summary>
    /// Notificación textual asociada a la línea o ruta.
    /// </summary>
    [JsonPropertyName("notificacion")]
    public string? Notificacion { get; init; }
}
