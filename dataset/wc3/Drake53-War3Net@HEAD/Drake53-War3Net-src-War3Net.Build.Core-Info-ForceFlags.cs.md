---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/ForceFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    [Flags]
    public enum ForceFlags
    {
        Allied = 1 << 0,
        AlliedVictory = 1 << 1,
        UNK4 = 1 << 2,
        ShareVision = 1 << 3,
        ShareUnitControl = 1 << 4,
        ShareAdvancedUnitControl = 1 << 5,
    }
}