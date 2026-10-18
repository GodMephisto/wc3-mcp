---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Info/RandomUnitTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class RandomUnitTable
    {
        internal RandomUnitTable(JsonElement jsonElement, MapInfoFormatVersion formatVersion)
        {
            GetFrom(jsonElement, formatVersion);
        }

        internal RandomUnitTable(ref Utf8JsonReader reader, MapInfoFormatVersion formatVersion)
        {
            ReadFrom(ref reader, formatVersion);
        }

        internal void GetFrom(JsonElement jsonElement, MapInfoFormatVersion formatVersion)
        {
            Index = jsonElement.GetInt32(nameof(Index));
            Name = jsonElement.GetString(nameof(Name));

            foreach (var element in jsonElement.EnumerateArray(nameof(Types)))
            {
                Types.Add(element.GetInt32<WidgetType>());
            }

            var typeCount = Types.Count;
            foreach (var element in jsonElement.EnumerateArray(nameof(UnitSets)))
            {
                UnitSets.Add(element.GetRandomUnitSet(formatVersion, typeCount));
            }
        }

        internal void ReadFrom(ref Utf8JsonReader reader, MapInfoFormatVersion formatVersion)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion);
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, MapInfoFormatVersion formatVersion)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Index), Index);
            writer.WriteString(nameof(Name), Name);

            writer.WriteObject(nameof(Types), Types, options);

            writer.WriteStartArray(nameof(UnitSets));
            foreach (var unitSet in UnitSets)
            {
                writer.Write(unitSet, options, formatVersion);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}