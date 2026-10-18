---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Info/CampaignMapButton.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class CampaignMapButton
    {
        internal CampaignMapButton(JsonElement jsonElement, CampaignInfoFormatVersion formatVersion)
        {
            GetFrom(jsonElement, formatVersion);
        }

        internal CampaignMapButton(ref Utf8JsonReader reader, CampaignInfoFormatVersion formatVersion)
        {
            ReadFrom(ref reader, formatVersion);
        }

        internal void GetFrom(JsonElement jsonElement, CampaignInfoFormatVersion formatVersion)
        {
            IsVisibleInitially = jsonElement.GetInt32(nameof(IsVisibleInitially));
            Chapter = jsonElement.GetString(nameof(Chapter));
            Title = jsonElement.GetString(nameof(Title));
            MapFilePath = jsonElement.GetString(nameof(MapFilePath));
        }

        internal void ReadFrom(ref Utf8JsonReader reader, CampaignInfoFormatVersion formatVersion)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion);
        }

        internal void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, CampaignInfoFormatVersion formatVersion)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(IsVisibleInitially), IsVisibleInitially);
            writer.WriteString(nameof(Chapter), Chapter);
            writer.WriteString(nameof(Title), Title);
            writer.WriteString(nameof(MapFilePath), MapFilePath);

            writer.WriteEndObject();
        }
    }
}