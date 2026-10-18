---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonDestructableObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonDestructableObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DestructableObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<DestructableObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override DestructableObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadDestructableObjectData();
            }

            public override void Write(Utf8JsonWriter writer, DestructableObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}