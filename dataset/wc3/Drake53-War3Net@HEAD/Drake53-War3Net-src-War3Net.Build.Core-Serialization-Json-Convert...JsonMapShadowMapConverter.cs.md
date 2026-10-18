---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonMapShadowMapConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonMapShadowMapConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(MapShadowMap);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<MapShadowMap>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override MapShadowMap? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.ReadMapShadowMap();
            }

            public override void Write(Utf8JsonWriter writer, MapShadowMap value, JsonSerializerOptions options)
            {
                writer.Write(value, options);
            }
        }
    }
}