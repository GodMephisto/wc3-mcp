---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Environment/MapShadowMap.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class MapShadowMap
    {
        internal MapShadowMap(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            Cells.AddRange(reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position)));
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write(Cells.ToArray());
        }
    }
}