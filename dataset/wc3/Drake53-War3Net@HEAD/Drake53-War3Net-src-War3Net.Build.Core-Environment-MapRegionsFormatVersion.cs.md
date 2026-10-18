---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/MapRegionsFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Environment
{
    /// <summary>
    /// File format version for <see cref="MapRegions"/>.
    /// </summary>
    public enum MapRegionsFormatVersion
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        v2 = 2,

        [EditorBrowsable(EditorBrowsableState.Never)]
        v3 = 3,

        /// <summary>The initial version.</summary>
        v5 = 5,

        /// <summary>Introduced in patch 3.0.0.</summary>
        v7 = 7,
    }
}