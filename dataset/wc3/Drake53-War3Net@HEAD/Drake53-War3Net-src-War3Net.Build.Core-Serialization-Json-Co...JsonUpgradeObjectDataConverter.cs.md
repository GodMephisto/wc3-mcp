---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonUpgradeObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonUpgradeObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(UpgradeObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<UpgradeObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override UpgradeObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadUpgradeObjectData();
            }

            public override void Write(Utf8JsonWriter writer, UpgradeObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}