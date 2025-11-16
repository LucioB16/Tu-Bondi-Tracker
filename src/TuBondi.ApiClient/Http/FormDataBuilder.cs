using System.Globalization;
using System.Net.Http;

namespace TuBondi.ApiClient.Http;

/// <summary>
/// Facilita la construcción de contenido <see cref="FormUrlEncodedContent"/> respetando codificación UTF-8 e invariante cultural.
/// </summary>
/// <remarks>
/// Este helper asegura que los valores numéricos se serialicen usando <see cref="CultureInfo.InvariantCulture"/> evitando desalineaciones regionales.
/// </remarks>
public sealed class FormDataBuilder
{
    private readonly List<KeyValuePair<string, string>> _values = new();

    /// <summary>
    /// Agrega un par clave-valor sin transformar, ignorando entradas nulas.
    /// </summary>
    /// <param name="key">Nombre del parámetro esperado por el backend.</param>
    /// <param name="value">Valor preformateado listo para enviarse.</param>
    /// <returns>Instancia de <see cref="FormDataBuilder"/> para facilitar encadenamiento.</returns>
    public FormDataBuilder Add(string key, string? value)
    {
        if (!string.IsNullOrEmpty(key) && value is not null)
        {
            _values.Add(new KeyValuePair<string, string>(key, value));
        }

        return this;
    }

    /// <summary>
    /// Agrega un valor numérico usando formato invariante.
    /// </summary>
    /// <param name="key">Nombre del parámetro.</param>
    /// <param name="value">Valor numérico a convertir.</param>
    /// <returns>Instancia reutilizable del builder.</returns>
    public FormDataBuilder Add(string key, IFormattable value) =>
        Add(key, value.ToString(null, CultureInfo.InvariantCulture));

    /// <summary>
    /// Genera el contenido codificado listo para enviar como cuerpo de una solicitud POST.</summary>
    /// <returns>Instancia nueva de <see cref="FormUrlEncodedContent"/> con las claves agregadas.</returns>
    /// <remarks>
    /// El contenido utiliza codificación UTF-8 de manera predeterminada tal como lo define <see cref="FormUrlEncodedContent"/>.
    /// </remarks>
    public FormUrlEncodedContent Build() => new(_values);
}
