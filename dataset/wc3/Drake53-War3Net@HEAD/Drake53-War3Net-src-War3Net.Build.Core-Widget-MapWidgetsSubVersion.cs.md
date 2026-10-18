---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/MapWidgetsSubVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Widget
{
    /// <summary>
    /// Secondary file format version for <see cref="MapDoodads"/> and <see cref="MapUnits"/>.
    /// </summary>
    public enum MapWidgetsSubVersion
    {
        v7 = 7,
        v9 = 9,
        v10 = 10,
        v11 = 11,
    }
}