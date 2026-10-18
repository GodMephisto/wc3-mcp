---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/CampaignFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    [Flags]
    public enum CampaignFlags
    {
        VariableDifficultyLevels = 1 << 0,
        RequiresExpansion = 1 << 1,
        UseMapMinimap = 1 << 2,
    }
}