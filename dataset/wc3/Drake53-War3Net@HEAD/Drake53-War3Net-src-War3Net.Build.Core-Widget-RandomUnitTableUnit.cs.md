---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/RandomUnitTableUnit.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitTableUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitTableUnit"/> class.
        /// </summary>
        public RandomUnitTableUnit()
        {
        }

        public int UnitId { get; set; }

        public int Chance { get; set; }

        public override string ToString() => $"{UnitId.ToRawcode()} ({Chance}%)";
    }
}