using System.Text.Json;
using System.Text.Json.Serialization;
using TuBondi.ApiClient.Models;

namespace TuBondi.ApiClient.Serialization;

/// <summary>
/// Convierte arreglos JSON de dos posiciones (<c>[lon, lat]</c>) a objetos <see cref="GeoPoint"/>.
/// </summary>
/// <example>
/// Entrada típica: <c>[-64.180402, -31.414503]</c>.
/// </example>
public sealed class GeoPointConverter : JsonConverter<GeoPoint>
{
    /// <inheritdoc />
    public override GeoPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Se esperaba un arreglo para representar un punto geográfico");
        }

        reader.Read();
        var lon = reader.GetDouble();
        reader.Read();
        var lat = reader.GetDouble();
        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("El arreglo de punto geográfico debe contener exactamente dos valores");
        }

        return new GeoPoint(lon, lat);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GeoPoint value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Lon);
        writer.WriteNumberValue(value.Lat);
        writer.WriteEndArray();
    }
}
