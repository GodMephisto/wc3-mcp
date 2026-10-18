---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonCampaignInfoConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonCampaignInfoConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(CampaignInfo);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<CampaignInfo>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override CampaignInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadCampaignInfo();
            }

            public override void Write(Utf8JsonWriter writer, CampaignInfo value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}