---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/RandomUnitSet.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class RandomUnitSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitSet"/> class.
        /// </summary>
        public RandomUnitSet()
        {
        }

        public int Chance { get; set; }

        public int[] UnitIds { get; set; }

        public override string ToString() => $"{string.Join(", ", UnitIds.Select(id => id.ToRawcode()))} ({Chance}%)";
    }
}