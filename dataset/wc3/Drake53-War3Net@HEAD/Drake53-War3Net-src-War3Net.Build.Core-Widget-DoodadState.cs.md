---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/DoodadState.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    [Flags]
    public enum DoodadState : byte
    {
        NonSolidInvisible = 0,
        NonSolidVisible = 1 << 0,
        Normal = 1 << 1,
        WithZ = 1 << 2,
        Flag7 = 1 << 6,
    }
}