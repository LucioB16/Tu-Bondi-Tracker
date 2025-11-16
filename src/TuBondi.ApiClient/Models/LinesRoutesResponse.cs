using System.Text.Json;
using System.Text.Json.Serialization;
using TuBondi.ApiClient.Serialization;

namespace TuBondi.ApiClient.Models;

/// <summary>
/// Representa la respuesta completa del comando <c>lineasyrutas</c>.
/// </summary>
public sealed class LinesRoutesResponse
{
    /// <summary>
    /// Obtiene las líneas disponibles junto con sus rutas activas.
    /// </summary>
    [JsonPropertyName("lineas")]
    public IReadOnlyList<Linea> Lineas { get; init; } = Array.Empty<Linea>();

    /// <summary>
    /// Obtiene el código de verificación entregado por el backend.
    /// </summary>
    [JsonPropertyName("lineascrc")]
    public string? LineasCrc { get; init; }

    /// <summary>
    /// Obtiene la colección de grupos comerciales.
    /// </summary>
    [JsonPropertyName("grupos")]
    public IReadOnlyList<Grupo> Grupos { get; init; } = Array.Empty<Grupo>();

    /// <summary>
    /// Obtiene la lista de clientes (empresas u operadores) configurados.
    /// </summary>
    [JsonPropertyName("clientes")]
    public IReadOnlyList<Cliente> Clientes { get; init; } = Array.Empty<Cliente>();

    /// <summary>
    /// Obtiene los datos crudos asociados a la clave <c>metro</c> que la API expone sin tipar.
    /// </summary>
    [JsonPropertyName("metro")]
    public IReadOnlyList<JsonElement> Metro { get; init; } = Array.Empty<JsonElement>();
}

/// <summary>
/// Describe una línea de transporte urbano.
/// </summary>
public sealed class Linea
{
    /// <summary>
    /// Identificador textual de la línea (tal como se muestra al usuario).
    /// </summary>
    [JsonPropertyName("linea_id")]
    public string LineaId { get; init; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo de la línea.
    /// </summary>
    [JsonPropertyName("linea_nombre")]
    public string LineaNombre { get; init; } = string.Empty;

    /// <summary>
    /// Nombre del grupo comercial al que pertenece la línea.
    /// </summary>
    [JsonPropertyName("grupo")]
    public string Grupo { get; init; } = string.Empty;

    /// <summary>
    /// Color hexadecimal sugerido para presentar la línea.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;

    /// <summary>
    /// Colección de rutas asociadas a la línea.
    /// </summary>
    [JsonPropertyName("rutas")]
    public IReadOnlyList<Ruta> Rutas { get; init; } = Array.Empty<Ruta>();

    /// <summary>
    /// Identificador numérico del cliente asociado.
    /// </summary>
    [JsonPropertyName("cliente")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Cliente { get; init; }

    /// <summary>
    /// Orden sugerido para ordenar visualmente las líneas.
    /// </summary>
    [JsonPropertyName("orden")]
    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public double Orden { get; init; }
}

/// <summary>
/// Describe una ruta específica perteneciente a una línea.
/// </summary>
public sealed class Ruta
{
    /// <summary>
    /// Identificador textual de la ruta.
    /// </summary>
    [JsonPropertyName("ruta_id")]
    public string RutaId { get; init; } = string.Empty;

    /// <summary>
    /// Descripción del sentido de circulación.
    /// </summary>
    [JsonPropertyName("sentido")]
    public string Sentido { get; init; } = string.Empty;

    /// <summary>
    /// Nombre completo de la ruta.
    /// </summary>
    [JsonPropertyName("ruta_nombre")]
    public string RutaNombre { get; init; } = string.Empty;

    /// <summary>
    /// Longitud de la ruta, reportada como cadena sin procesar.
    /// </summary>
    [JsonPropertyName("longitud")]
    public string Longitud { get; init; } = string.Empty;
}

/// <summary>
/// Describe un grupo de líneas.
/// </summary>
public sealed class Grupo
{
    /// <summary>
    /// Identificador del cliente al que pertenece el grupo.
    /// </summary>
    [JsonPropertyName("cliente_id")]
    public string? ClienteId { get; init; }

    /// <summary>
    /// Identificador del grupo dentro del cliente.
    /// </summary>
    [JsonPropertyName("grupo_id")]
    public string? GrupoId { get; init; }

    /// <summary>
    /// Nombre amigable del grupo.
    /// </summary>
    [JsonPropertyName("nombre")]
    public string? Nombre { get; init; }

    /// <summary>
    /// Número de troncal o referencia textual.
    /// </summary>
    [JsonPropertyName("troncal")]
    public string? Troncal { get; init; }
}

/// <summary>
/// Describe un cliente (empresa prestataria).
/// </summary>
public sealed class Cliente
{
    /// <summary>
    /// Identificador numérico del cliente.
    /// </summary>
    [JsonPropertyName("cliente_id")]
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int ClienteId { get; init; }

    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    [JsonPropertyName("nombre")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Jurisdicciones habilitadas para el cliente.
    /// </summary>
    [JsonPropertyName("jurisdicciones")]
    public string Jurisdicciones { get; init; } = string.Empty;

    /// <summary>
    /// Color representativo provisto por la API.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;

    /// <summary>
    /// Indica si la API expone rutas detalladas para el cliente.
    /// </summary>
    [JsonPropertyName("usa_ruta")]
    public bool UsaRuta { get; init; }

    /// <summary>
    /// Valor crudo que describe la demora predeterminada de coches.
    /// </summary>
    [JsonPropertyName("demora_de_coches")]
    public string DemoraDeCoches { get; init; } = string.Empty;

    /// <summary>
    /// Bandera textual que indica si la línea es circular.
    /// </summary>
    [JsonPropertyName("lin_circular")]
    public string LinCircular { get; init; } = string.Empty;

    /// <summary>
    /// Indica si el diagramado debe ocultarse; la API puede omitir este campo.
    /// </summary>
    [JsonPropertyName("no_mostrar_diagramado")]
    public bool? NoMostrarDiagramado { get; init; }
}
