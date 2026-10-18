---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/MapCustomTextTriggersFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable SA1300

namespace War3Net.Build.Script
{
    /// <summary>
    /// File format version for <see cref="MapCustomTextTriggers"/>.
    /// </summary>
    public enum MapCustomTextTriggersFormatVersion
    {
        /// <summary>Reign of Chaos format.</summary>
        v0 = 0,

        /// <summary>The Frozen Throne format.</summary>
        v1 = 1,
    }
}