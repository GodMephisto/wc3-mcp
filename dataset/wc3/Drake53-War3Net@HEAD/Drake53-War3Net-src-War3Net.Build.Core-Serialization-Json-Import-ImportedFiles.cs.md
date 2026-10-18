---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Json/Import/ImportedFiles.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Import
{
    [JsonConverter(typeof(JsonImportedFilesConverter))]
    public sealed partial class ImportedFiles
    {
        internal ImportedFiles(JsonElement jsonElement)
        {
            GetFrom(jsonElement);
        }

        internal ImportedFiles(ref Utf8JsonReader reader)
        {
            ReadFrom(ref reader);
        }

        internal void GetFrom(JsonElement jsonElement)
        {
            FormatVersion = jsonElement.GetInt32<ImportedFilesFormatVersion>(nameof(FormatVersion));

            foreach (var element in jsonElement.EnumerateArray(nameof(Files)))
            {
                Files.Add(element.GetImportedFile(FormatVersion));
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

            writer.WriteStartArray(nameof(Files));
            foreach (var file in Files)
            {
                writer.Write(file, options, FormatVersion);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}