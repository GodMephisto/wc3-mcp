---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Configuration/GameConfigurationFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Configuration
{
    [Flags]
    public enum GameConfigurationFlags
    {
        IsFogOfWarDisabled = 1 << 0,
        IsVictoryDefeatConditionsDisabled = 1 << 1,
    }
}