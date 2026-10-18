---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/SupportedModes.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    [Flags]
    public enum SupportedModes
    {
        /// <summary>Classic graphics.</summary>
        SD = 1 << 0,

        /// <summary>Reforged graphics.</summary>
        HD = 1 << 1,

        /// <summary>Definitive Edition graphics.</summary>
        DE = 1 << 2,
    }
}