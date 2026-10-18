---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/ItemClass.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public enum ItemClass : byte
    {
        Permanent = 0,
        Charged = 1,
        PowerUp = 2,
        Artifact = 3,
        Purchasable = 4,
        Campaign = 5,
        Miscellaneous = 6,

        Any = 8,
    }
}