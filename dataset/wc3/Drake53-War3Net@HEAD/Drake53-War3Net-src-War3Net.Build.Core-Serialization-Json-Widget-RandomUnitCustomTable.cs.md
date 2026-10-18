---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Widget/RandomUnitCustomTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitCustomTable : RandomUnitData
    {
        internal RandomUnitCustomTable(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(jsonElement, formatVersion, subVersion, useNewFormat);
        }

        internal RandomUnitCustomTable(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            ReadFrom(ref reader, formatVersion, subVersion, useNewFormat);
        }

        internal void GetFrom(JsonElement jsonElement, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            foreach (var element in jsonElement.EnumerateArray(nameof(RandomUnits)))
            {
                RandomUnits.Add(element.GetRandomUnitTableUnit(formatVersion, subVersion, useNewFormat));
            }
        }

        internal void ReadFrom(ref Utf8JsonReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion, subVersion, useNewFormat);
        }

        internal override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            writer.WriteStartObject();

            writer.WriteStartArray(nameof(RandomUnits));
            foreach (var randomUnit in RandomUnits)
            {
                writer.Write(randomUnit, options, formatVersion, subVersion, useNewFormat);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}