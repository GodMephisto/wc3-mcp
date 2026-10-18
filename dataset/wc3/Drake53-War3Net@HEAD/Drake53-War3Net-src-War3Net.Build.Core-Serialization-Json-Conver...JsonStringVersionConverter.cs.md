---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Converters/JsonStringVersionConverter.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Serialization.Json
{
    internal sealed class JsonStringVersionConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Version);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new Converter(options);
        }

        private class Converter : JsonConverter<Version>
        {
            public Converter(JsonSerializerOptions options)
            {
            }

            public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                var versionString = reader.GetString();
                return versionString is null ? null : new Version(versionString);
            }

            public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}