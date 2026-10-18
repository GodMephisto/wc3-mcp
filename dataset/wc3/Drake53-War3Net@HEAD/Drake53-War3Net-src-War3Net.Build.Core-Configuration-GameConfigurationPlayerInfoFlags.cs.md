---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Configuration/GameConfigurationPlayerInfoFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Configuration
{
    [Flags]
    public enum GameConfigurationPlayerInfoFlags
    {
        IsUser = 1 << 0,
        IsObserver = 1 << 1,
        LoadCustomAIFile = 1 << 2,
        AIFilePathIsAbsolute = 1 << 3,
    }
}