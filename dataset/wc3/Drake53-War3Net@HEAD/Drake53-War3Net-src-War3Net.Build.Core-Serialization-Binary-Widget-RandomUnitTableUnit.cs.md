---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Widget/RandomUnitTableUnit.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitTableUnit
    {
        internal RandomUnitTableUnit(BinaryReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            ReadFrom(reader, formatVersion, subVersion, useNewFormat);
        }

        internal void ReadFrom(BinaryReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            UnitId = reader.ReadInt32();
            Chance = reader.ReadInt32();
        }

        internal void WriteTo(BinaryWriter writer, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            writer.Write(UnitId);
            writer.Write(Chance);
        }
    }
}