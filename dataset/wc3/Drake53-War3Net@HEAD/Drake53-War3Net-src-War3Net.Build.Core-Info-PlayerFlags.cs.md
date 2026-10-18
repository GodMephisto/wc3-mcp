---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/PlayerFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    [Flags]
    public enum PlayerFlags
    {
        FixedStartPosition = 1 << 0,
        RaceSelectable = 1 << 1,
    }
}