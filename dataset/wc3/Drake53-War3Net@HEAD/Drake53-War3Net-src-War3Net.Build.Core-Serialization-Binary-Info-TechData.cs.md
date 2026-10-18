---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Info/TechData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class TechData
    {
        internal TechData(BinaryReader reader, MapInfoFormatVersion formatVersion)
        {
            ReadFrom(reader, formatVersion);
        }

        internal void ReadFrom(BinaryReader reader, MapInfoFormatVersion formatVersion)
        {
            Players = reader.ReadBitmask32();
            Id = reader.ReadInt32();
        }

        internal void WriteTo(BinaryWriter writer, MapInfoFormatVersion formatVersion)
        {
            writer.Write(Players);
            writer.Write(Id);
        }
    }
}