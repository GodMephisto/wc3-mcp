---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/ObjectDataType.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public enum ObjectDataType
    {
        Int = 0,
        Real = 1,
        Unreal = 2,
        String = 3,
        [Obsolete] Bool = 4,
        [Obsolete] Char = 5,
    }
}