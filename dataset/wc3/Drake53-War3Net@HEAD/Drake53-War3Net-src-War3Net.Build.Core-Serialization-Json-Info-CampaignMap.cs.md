---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Info/CampaignMap.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class CampaignMap
    {
        internal CampaignMap(JsonElement jsonElement, CampaignInfoFormatVersion formatVersion)
        {
            GetFrom(jsonElement, formatVersion);
        }

        internal CampaignMap(ref Utf8JsonReader reader, CampaignInfoFormatVersion formatVersion)
        {
            ReadFrom(ref reader, formatVersion);
        }

        internal void GetFrom(JsonElement jsonElement, CampaignInfoFormatVersion formatVersion)
        {
            Unk = jsonElement.GetString(nameof(Unk));
            MapFilePath = jsonElement.GetString(nameof(MapFilePath));
        }

        internal void ReadFrom(ref Utf8JsonReader reader, CampaignInfoFormatVersion formatVersion)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion);
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, CampaignInfoFormatVersion formatVersion)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Unk), Unk);
            writer.WriteString(nameof(MapFilePath), MapFilePath);

            writer.WriteEndObject();
        }
    }
}