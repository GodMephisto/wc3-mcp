---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Info/RandomItemTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class RandomItemTable
    {
        internal RandomItemTable(BinaryReader reader, MapInfoFormatVersion formatVersion)
        {
            ReadFrom(reader, formatVersion);
        }

        internal void ReadFrom(BinaryReader reader, MapInfoFormatVersion formatVersion)
        {
            Index = reader.ReadInt32();
            Name = reader.ReadChars();

            nint itemSetCount = reader.ReadInt32();
            for (nint i = 0; i < itemSetCount; i++)
            {
                ItemSets.Add(reader.ReadRandomItemSet(formatVersion));
            }
        }

        internal void WriteTo(BinaryWriter writer, MapInfoFormatVersion formatVersion)
        {
            writer.Write(Index);
            writer.WriteString(Name);

            writer.Write(ItemSets.Count);
            foreach (var itemSet in ItemSets)
            {
                writer.Write(itemSet, formatVersion);
            }
        }
    }
}