---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Audio/MapSounds.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Audio
{
    [JsonConverter(typeof(JsonMapSoundsConverter))]
    public sealed partial class MapSounds
    {
        internal MapSounds(JsonElement jsonElement)
        {
            GetFrom(jsonElement);
        }

        internal MapSounds(ref Utf8JsonReader reader)
        {
            ReadFrom(ref reader);
        }

        internal void GetFrom(JsonElement jsonElement)
        {
            FormatVersion = jsonElement.GetInt32<MapSoundsFormatVersion>(nameof(FormatVersion));

            foreach (var element in jsonElement.EnumerateArray(nameof(Sounds)))
            {
                Sounds.Add(element.GetSound(FormatVersion));
            }
        }

        internal void ReadFrom(ref Utf8JsonReader reader)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement);
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteObject(nameof(FormatVersion), FormatVersion, options);

            writer.WriteStartArray(nameof(Sounds));
            foreach (var sound in Sounds)
            {
                writer.Write(sound, options, FormatVersion);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}