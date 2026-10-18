---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonAbilityObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonAbilityObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(AbilityObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<AbilityObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override AbilityObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadAbilityObjectData();
            }

            public override void Write(Utf8JsonWriter writer, AbilityObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}