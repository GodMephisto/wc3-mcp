---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Configuration/GameConfigurationFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Configuration
{
    /// <summary>
    /// File format version for <see cref="GameConfiguration"/>.
    /// </summary>
    public enum GameConfigurationFormatVersion
    {
        /// <summary>The initial version.</summary>
        v1 = 1,
    }
}