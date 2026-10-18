---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Common/Bitmask32.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Common
{
    public sealed partial class Bitmask32
    {
        internal Bitmask32(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            _mask = reader.ReadInt32();
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(_mask);
        }
    }
}