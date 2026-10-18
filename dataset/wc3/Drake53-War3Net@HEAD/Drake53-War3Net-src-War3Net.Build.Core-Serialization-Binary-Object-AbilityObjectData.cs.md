---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Object/AbilityObjectData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class AbilityObjectData
    {
        internal AbilityObjectData(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            FormatVersion = reader.ReadInt32<ObjectDataFormatVersion>();

            nint baseAbilitiesCount = reader.ReadInt32();
            for (nint i = 0; i < baseAbilitiesCount; i++)
            {
                BaseAbilities.Add(reader.ReadLevelObjectModification(FormatVersion));
            }

            nint newAbilitiesCount = reader.ReadInt32();
            for (nint i = 0; i < newAbilitiesCount; i++)
            {
                NewAbilities.Add(reader.ReadLevelObjectModification(FormatVersion));
            }
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((int)FormatVersion);

            writer.Write(BaseAbilities.Count);
            foreach (var ability in BaseAbilities)
            {
                writer.Write(ability, FormatVersion);
            }

            writer.Write(NewAbilities.Count);
            foreach (var ability in NewAbilities)
            {
                writer.Write(ability, FormatVersion);
            }
        }
    }
}