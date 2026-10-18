---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonItemObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonItemObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(ItemObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<ItemObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override ItemObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadItemObjectData();
            }

            public override void Write(Utf8JsonWriter writer, ItemObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}