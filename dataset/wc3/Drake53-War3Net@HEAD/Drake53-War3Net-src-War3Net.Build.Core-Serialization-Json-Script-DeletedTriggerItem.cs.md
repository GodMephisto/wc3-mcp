---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Script/DeletedTriggerItem.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class DeletedTriggerItem : TriggerItem
    {
        internal DeletedTriggerItem(JsonElement jsonElement, TriggerItemType triggerItemType, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion)
            : base(triggerItemType)
        {
            GetFrom(jsonElement, formatVersion, subVersion);
        }

        internal DeletedTriggerItem(ref Utf8JsonReader reader, TriggerItemType triggerItemType, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion)
            : base(triggerItemType)
        {
            ReadFrom(ref reader, formatVersion, subVersion);
        }

        internal void GetFrom(JsonElement jsonElement, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion)
        {
            Id = jsonElement.GetInt32(nameof(Id));

            Name = "<DELETED>";
            ParentId = -1;
        }

        internal void ReadFrom(ref Utf8JsonReader reader, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion)
        {
            GetFrom(JsonDocument.ParseValue(ref reader).RootElement, formatVersion, subVersion);
        }

        internal override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion)
        {
            writer.WriteStartObject();

            writer.WriteObject(nameof(Type), Type, options);

            if (subVersion is not null)
            {
                writer.WriteNumber(nameof(Id), Id);
            }

            writer.WriteEndObject();
        }
    }
}