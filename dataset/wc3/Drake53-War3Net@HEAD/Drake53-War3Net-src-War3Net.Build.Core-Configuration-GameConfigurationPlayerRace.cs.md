---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Configuration/GameConfigurationPlayerRace.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Configuration
{
    public enum GameConfigurationPlayerRace
    {
        Human = 1 << 0,
        Orc = 1 << 1,
        NightElf = 1 << 2,
        Undead = 1 << 3,
        Random = 1 << 5,
    }
}