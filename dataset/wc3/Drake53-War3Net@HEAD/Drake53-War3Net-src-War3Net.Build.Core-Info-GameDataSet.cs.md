---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/GameDataSet.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public enum GameDataSet
    {
        /// <summary>
        /// Based on map melee status.
        /// </summary>
        Default = 0,

        /// <summary>
        /// RoC 1.01, Tft 1.07.
        /// </summary>
        Custom = 1,

        /// <summary>
        /// Latest patch.
        /// </summary>
        Melee = 2,

        Unset = -1,
    }
}