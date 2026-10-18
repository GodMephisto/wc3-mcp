---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/MaterialFlags.cs"
ref: HEAD
---

﻿using System;

namespace BananaMpq.Geometry
{
    [Flags]
    public enum MaterialFlags : ushort
    {
        None = 0,
        Liquid = 1,
        DamageOverTime = 2,
        Traversable = 32768,
    }
}