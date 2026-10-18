---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/MapCustomTextTriggersSubVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Script
{
    /// <summary>
    /// Secondary file format version for <see cref="MapCustomTextTriggers"/>.
    /// </summary>
    public enum MapCustomTextTriggersSubVersion
    {
        /// <summary>The initial beta version.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        v1 = unchecked((int)0x80000001),

        /// <summary>The initial version.</summary>
        v4 = unchecked((int)0x80000004),
    }
}