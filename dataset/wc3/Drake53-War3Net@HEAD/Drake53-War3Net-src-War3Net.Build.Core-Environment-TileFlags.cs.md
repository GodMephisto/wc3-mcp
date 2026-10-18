---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/TileFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    [Flags]
    public enum TileFlags : byte
    {
        Ramp = 1 << 0,
        Blighted = 1 << 1,
        Water = 1 << 2,
        Boundary = 1 << 3,
    }
}