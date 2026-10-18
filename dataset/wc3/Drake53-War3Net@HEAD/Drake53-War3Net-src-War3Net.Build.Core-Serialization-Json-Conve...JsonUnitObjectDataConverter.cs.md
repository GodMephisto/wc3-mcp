---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonUnitObjectDataConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonUnitObjectDataConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(UnitObjectData);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<UnitObjectData>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override UnitObjectData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadUnitObjectData();
            }

            public override void Write(Utf8JsonWriter writer, UnitObjectData value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}