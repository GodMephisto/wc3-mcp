---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/PathingType.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    [Flags]
    public enum PathingType : byte
    {
        Walk = 1 << 1,
        Fly = 1 << 2,
        Build = 1 << 3,

        Blight = 1 << 5,
        Water = 1 << 6,
        UNK = 1 << 7,
    }
}