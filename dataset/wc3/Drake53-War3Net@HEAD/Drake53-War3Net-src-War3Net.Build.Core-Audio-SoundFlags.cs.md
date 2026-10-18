---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Audio/SoundFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Audio
{
    [Flags]
    public enum SoundFlags
    {
        Looping = 1 << 0,
        Is3DSound = 1 << 1,
        StopWhenOutOfRange = 1 << 2,
        Music = 1 << 3,
        UNK16 = 1 << 4,

        // flags from .slk files:
        // WANT3D
        // IGNOREUSERNAME
        // CHANNELFULLPREEMPT
        // NODUPLICATES
        // SCALEPRIORITY
        // LOOPING
        // RANDOMPITCH
        // CHANNELFULLPREEMPTOLDEST
        // DYNAMICOCCLUSION
        // LISTFULLPREEMPT
    }
}