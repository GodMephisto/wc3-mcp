---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Widget/RandomUnitAny.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitAny : RandomUnitData
    {
        internal RandomUnitAny(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(jsonElement, formatVersion, subVersion, useNewFormat);
        }

        internal RandomUnitAny(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            ReadFrom(ref reader, formatVersion, subVersion, useNewFormat);
        }

        internal void GetFrom(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            Level = jsonElement.GetInt32(nameof(Level));
            Class = jsonElement.GetByte<ItemClass>(nameof(Class));
        }

        internal void ReadFrom(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion, subVersion, useNewFormat);
        }

        internal override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Level), Level);
            writer.WriteObject(nameof(Class), Class, options);

            writer.WriteEndObject();
        }
    }
}