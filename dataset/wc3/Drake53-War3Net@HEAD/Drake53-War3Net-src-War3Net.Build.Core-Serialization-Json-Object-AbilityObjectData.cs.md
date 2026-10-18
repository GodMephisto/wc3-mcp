---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Object/AbilityObjectData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    [JsonConverter(typeof(JsonAbilityObjectDataConverter))]
    public sealed partial class AbilityObjectData
    {
        internal AbilityObjectData(JsonElement jsonElement)
        {
            GetFrom(jsonElement);
        }

        internal AbilityObjectData(ref Utf8JsonReader reader)
        {
            ReadFrom(ref reader);
        }

        internal void GetFrom(JsonElement jsonElement)
        {
            FormatVersion = jsonElement.GetInt32<ObjectDataFormatVersion>(nameof(FormatVersion));

            foreach (var element in jsonElement.EnumerateArray(nameof(BaseAbilities)))
            {
                BaseAbilities.Add(element.GetLevelObjectModification(FormatVersion));
            }

            foreach (var element in jsonElement.EnumerateArray(nameof(NewAbilities)))
            {
                NewAbilities.Add(element.GetLevelObjectModification(FormatVersion));
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

            writer.WriteStartArray(nameof(BaseAbilities));
            foreach (var ability in BaseAbilities)
            {
                writer.Write(ability, options, FormatVersion);
            }

            writer.WriteEndArray();

            writer.WriteStartArray(nameof(NewAbilities));
            foreach (var ability in NewAbilities)
            {
                writer.Write(ability, options, FormatVersion);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}