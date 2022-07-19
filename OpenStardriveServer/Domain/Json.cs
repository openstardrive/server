using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain;

public static class Json
{
    private static JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
        
    public static string Serialize(object input)
    {
        return JsonSerializer.Serialize(input, options);
    }

    public static T Deserialize<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, options);
    }
}
    
public class RawJsonWriter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString();

    public override void Write(Utf8JsonWriter writer, string stringValue, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.Parse(stringValue);
        document.WriteTo(writer);
    }
}