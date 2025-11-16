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
    /// Detalles adicionales de error proporcionados por el backend; es <see langword="null"/> cuando no se reportan advertencias.
    /// </summary>
    [JsonPropertyName("error")]
    public IReadOnlyList<string>? Error { get; init; }
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
    /// Sentido operativo reportado en el horario.
    /// </summary>
    [JsonPropertyName("sentido")]
    public string? Sentido { get; init; }

    /// <summary>
    /// Ruta en la que circula actualmente.
    /// </summary>
    [JsonPropertyName("ruta")]
    public string? Ruta { get; init; }

    /// <summary>
    /// Próxima ruta asociada según el backend.
    /// </summary>
    [JsonPropertyName("ruta_siguiente")]
    public string? RutaSiguiente { get; init; }

    /// <summary>
    /// Identificador del servicio en ejecución.
    /// </summary>
    [JsonPropertyName("servicio")]
    public string? Servicio { get; init; }

    /// <summary>
    /// Número de media vuelta reportado.
    /// </summary>
    [JsonPropertyName("media_vuelta")]
    public string? MediaVuelta { get; init; }

    /// <summary>
    /// Código del itinerario actual.
    /// </summary>
    [JsonPropertyName("itinerario_codigo")]
    public string? ItinerarioCodigo { get; init; }

    /// <summary>
    /// Marca temporal del itinerario en formato <c>yyyy-MM-dd HH:mm:ss</c>.
    /// </summary>
    [JsonPropertyName("itinerario_fechayhora")]
    public string? ItinerarioFechaYHora { get; init; }

    /// <summary>
    /// Cliente u operador asociado.
    /// </summary>
    [JsonPropertyName("cliente")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Cliente { get; init; }

    /// <summary>
    /// Demora informada en minutos. El valor centinela <c>99999</c> indica que la demora es desconocida.
    /// </summary>
    [JsonPropertyName("demora_minutos")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double DemoraMinutos { get; init; }

    /// <summary>
    /// Demora textual (por ejemplo <c>"14min"</c>) si está disponible.
    /// </summary>
    [JsonPropertyName("demora")]
    public string? Demora { get; init; }

    /// <summary>
    /// Velocidad aproximada en kilómetros por hora.
    /// </summary>
    [JsonPropertyName("velocidad")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Velocidad { get; init; }

    /// <summary>
    /// Marca de tiempo en formato <c>+/-HH:MM:SS</c> devuelta por el backend.
    /// </summary>
    [JsonPropertyName("tiempo")]
    public string? Tiempo { get; init; }

    /// <summary>
    /// Prioridad de pantalla o display configurado para el servicio.
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
    /// Identificador interno auxiliar (<c>horario_sentido</c>).
    /// </summary>
    [JsonPropertyName("horario_sentido")]
    public string? HorarioSentido { get; init; }

    /// <summary>
    /// Valor auxiliar <c>horario_media_vuelta</c> asociado al plan.
    /// </summary>
    [JsonPropertyName("horario_media_vuelta")]
    public string? HorarioMediaVuelta { get; init; }

    /// <summary>
    /// Color sugerido para representar el coche.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Comentario libre enviado por el backend.
    /// </summary>
    [JsonPropertyName("comentario")]
    public string? Comentario { get; init; }

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

    /// <summary>
    /// Hora teórica ajustada en formato <c>HH:mm:ss</c>, cuando la API la provee.
    /// </summary>
    [JsonPropertyName("horaTeoricaAjustada")]
    public string? HoraTeoricaAjustada { get; init; }
}
