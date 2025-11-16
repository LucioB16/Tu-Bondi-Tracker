using System.Text.Json;
using System.Text.Json.Serialization;
using TuBondi.ApiClient.Models;

namespace TuBondi.ApiClient.Serialization;

/// <summary>
/// Convierte arreglos JSON de tres posiciones (<c>[lon, lat, course]</c>) a objetos <see cref="TrazaPoint"/>.
/// </summary>
/// <example>
/// Entrada típica: <c>[-64.1918, -31.4201, 90]</c>.
/// </example>
public sealed class TrazaPointConverter : JsonConverter<TrazaPoint>
{
    /// <inheritdoc />
    public override TrazaPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Se esperaba un arreglo para representar la traza");
        }

        reader.Read();
        var lon = reader.GetDouble();
        reader.Read();
        var lat = reader.GetDouble();
        reader.Read();
        var course = reader.GetDouble();
        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("El arreglo de traza debe contener exactamente tres valores");
        }

        return new TrazaPoint(lon, lat, course);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TrazaPoint value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Lon);
        writer.WriteNumberValue(value.Lat);
        writer.WriteNumberValue(value.Course);
        writer.WriteEndArray();
    }
}
