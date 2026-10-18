---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Widget/ModifiedAbilityData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class ModifiedAbilityData
    {
        internal ModifiedAbilityData(BinaryReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            ReadFrom(reader, formatVersion, subVersion, useNewFormat);
        }

        internal void ReadFrom(BinaryReader reader, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            AbilityId = reader.ReadInt32();
            IsAutocastActive = reader.ReadBool();
            HeroAbilityLevel = reader.ReadInt32();
        }

        internal void WriteTo(BinaryWriter writer, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat)
        {
            writer.Write(AbilityId);
            writer.WriteBool(IsAutocastActive);
            writer.Write(HeroAbilityLevel);
        }
    }
}