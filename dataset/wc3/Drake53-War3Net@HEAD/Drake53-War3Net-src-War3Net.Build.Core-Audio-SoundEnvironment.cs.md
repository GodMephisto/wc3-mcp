---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Audio/SoundEnvironment.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Audio
{
    [Obsolete]
    // NOTE: this is similar case as Sound.EAXSetting, where it's stored as string, but in WE it's limited to the options in a dropdown.
    public enum SoundEnvironment
    {
        Mountains,
        Lake,
        Psychotic,
        Dungeon,
    }
}