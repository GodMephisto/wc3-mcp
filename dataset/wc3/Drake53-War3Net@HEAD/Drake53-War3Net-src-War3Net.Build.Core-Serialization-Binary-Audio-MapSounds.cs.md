---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Audio/MapSounds.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Audio
{
    public sealed partial class MapSounds
    {
        internal MapSounds(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            FormatVersion = reader.ReadInt32<MapSoundsFormatVersion>();

            nint soundCount = reader.ReadInt32();
            for (nint i = 0; i < soundCount; i++)
            {
                Sounds.Add(reader.ReadSound(FormatVersion));
            }
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((int)FormatVersion);

            writer.Write(Sounds.Count);
            foreach (var sound in Sounds)
            {
                writer.Write(sound, FormatVersion);
            }
        }
    }
}