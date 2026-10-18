---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Common/Bitmask32.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Common
{
    public sealed partial class Bitmask32
    {
        internal Bitmask32(JsonElement jsonElement)
        {
            GetFrom(jsonElement);
        }

        internal Bitmask32(ref Utf8JsonReader reader)
        {
            ReadFrom(ref reader);
        }

        internal void GetFrom(JsonElement jsonElement)
        {
            _mask = jsonElement.GetInt32();
        }

        internal void ReadFrom(ref Utf8JsonReader reader)
        {
            _mask = reader.GetInt32();
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(_mask);
        }
    }
}