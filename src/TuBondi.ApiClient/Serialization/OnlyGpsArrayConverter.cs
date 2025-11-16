using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuBondi.ApiClient.Serialization;

/// <summary>
/// Convierte el campo <c>onlygps_array</c> que ocasionalmente llega como arreglo vacío.
/// </summary>
internal sealed class OnlyGpsArrayConverter : JsonConverter<IDictionary<string, bool>>
{
    public override IDictionary<string, bool> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, bool>>(ref reader, options);
            return dictionary ?? new Dictionary<string, bool>();
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            if (!reader.Read())
            {
                throw new JsonException("No fue posible leer el arreglo onlygps_array");
            }

            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException("onlygps_array fue devuelto como arreglo pero no está vacío");
            }

            return new Dictionary<string, bool>();
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            return new Dictionary<string, bool>();
        }

        throw new JsonException($"Tipo JSON inesperado para onlygps_array: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, IDictionary<string, bool> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
