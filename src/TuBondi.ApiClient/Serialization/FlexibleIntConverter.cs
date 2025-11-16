using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuBondi.ApiClient.Serialization;

/// <summary>
/// Convierte valores JSON que pueden venir como número o cadena hacia <see cref="int"/>.
/// </summary>
/// <example>
/// Entradas válidas: <c>1</c>, <c>"2"</c>, <c>"003"</c>.
/// </example>
public sealed class FlexibleIntConverter : JsonConverter<int>
{
    /// <inheritdoc />
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => int.Parse(reader.GetString()!, CultureInfo.InvariantCulture),
            JsonTokenType.Null => 0,
            _ => throw new JsonException($"Tipo inesperado {reader.TokenType} para int")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value);
}
