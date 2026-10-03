using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wc3.Commands;

/// <summary>
/// Writes a placed unit's or doodad's (Sx, Sy, Sz) scale tuple as {"sx", "sy", "sz"}.
/// </summary>
/// <remarks>
/// A C# tuple keeps its element names only at compile time, so the serializer saw a bare
/// ValueTuple. By default it skipped the tuple's fields and printed {}, which hid every scale from
/// MCP clients and from --json. Including fields printed item1, item2 and item3. This keeps the
/// names the code uses. Shared by the CLI and the MCP server so both print the same shape.
/// </remarks>
public sealed class ScaleJsonConverter : JsonConverter<(float Sx, float Sy, float Sz)>
{
    public override (float Sx, float Sy, float Sz) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        float sx = 1, sy = 1, sz = 1;
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("expected a scale object");
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string name = reader.GetString() ?? "";
            reader.Read();
            float v = reader.GetSingle();
            switch (name.ToLowerInvariant())
            {
                case "sx": sx = v; break;
                case "sy": sy = v; break;
                case "sz": sz = v; break;
            }
        }
        return (sx, sy, sz);
    }

    public override void Write(Utf8JsonWriter writer, (float Sx, float Sy, float Sz) value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("sx", value.Sx);
        writer.WriteNumber("sy", value.Sy);
        writer.WriteNumber("sz", value.Sz);
        writer.WriteEndObject();
    }
}
