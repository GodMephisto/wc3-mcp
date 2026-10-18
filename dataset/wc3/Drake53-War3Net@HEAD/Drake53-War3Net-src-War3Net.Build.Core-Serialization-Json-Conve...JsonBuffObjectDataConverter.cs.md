---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonBuffObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonBuffObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(BuffObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<BuffObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override BuffObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadBuffObjectData();
            }

            public override void Write(Utf8JsonWriter writer, BuffObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}