using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuBondi.ApiClient.Serialization;

/// <summary>
/// Convierte valores JSON que pueden ser números o cadenas hacia <see cref="double"/>.
/// </summary>
/// <example>
/// Entradas válidas: <c>84</c>, <c>84.5</c>, <c>"-31.411505"</c>.
/// </example>
public sealed class FlexibleDoubleConverter : JsonConverter<double>
{
    /// <inheritdoc />
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => double.Parse(reader.GetString()!, CultureInfo.InvariantCulture),
            JsonTokenType.Null => 0d,
            _ => throw new JsonException($"Tipo inesperado {reader.TokenType} para double")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value);
}
