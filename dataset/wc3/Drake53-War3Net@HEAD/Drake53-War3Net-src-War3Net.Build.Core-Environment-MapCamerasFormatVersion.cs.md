---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/MapCamerasFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable SA1300

namespace War3Net.Build.Environment
{
    /// <summary>
    /// File format version for <see cref="MapCameras"/>.
    /// </summary>
    public enum MapCamerasFormatVersion
    {
        /// <summary>The initial version.</summary>
        v0 = 0,

        /// <summary>Introduced in patch 3.0.0.</summary>
        v3 = 3,
    }
}