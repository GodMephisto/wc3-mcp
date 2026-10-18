---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Object/UpgradeObjectData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class UpgradeObjectData
    {
        internal UpgradeObjectData(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            FormatVersion = reader.ReadInt32<ObjectDataFormatVersion>();

            nint baseUpgradesCount = reader.ReadInt32();
            for (nint i = 0; i < baseUpgradesCount; i++)
            {
                BaseUpgrades.Add(reader.ReadLevelObjectModification(FormatVersion));
            }

            nint newUpgradesCount = reader.ReadInt32();
            for (nint i = 0; i < newUpgradesCount; i++)
            {
                NewUpgrades.Add(reader.ReadLevelObjectModification(FormatVersion));
            }
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((int)FormatVersion);

            writer.Write(BaseUpgrades.Count);
            foreach (var upgrade in BaseUpgrades)
            {
                writer.Write(upgrade, FormatVersion);
            }

            writer.Write(NewUpgrades.Count);
            foreach (var upgrade in NewUpgrades)
            {
                writer.Write(upgrade, FormatVersion);
            }
        }
    }
}