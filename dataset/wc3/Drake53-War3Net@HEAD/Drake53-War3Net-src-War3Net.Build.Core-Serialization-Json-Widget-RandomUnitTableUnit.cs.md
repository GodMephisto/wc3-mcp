---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Widget/RandomUnitTableUnit.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitTableUnit
    {
        internal RandomUnitTableUnit(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(jsonElement, formatVersion, subVersion, useNewFormat);
        }

        internal RandomUnitTableUnit(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            ReadFrom(ref reader, formatVersion, subVersion, useNewFormat);
        }

        internal void GetFrom(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            UnitId = jsonElement.GetInt32(nameof(UnitId));
            Chance = jsonElement.GetInt32(nameof(Chance));
        }

        internal void ReadFrom(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion, subVersion, useNewFormat);
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(UnitId), UnitId);
            writer.WriteNumber(nameof(Chance), Chance);

            writer.WriteEndObject();
        }
    }
}