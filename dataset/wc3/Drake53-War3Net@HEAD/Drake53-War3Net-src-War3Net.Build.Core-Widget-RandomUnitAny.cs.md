---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/RandomUnitAny.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitAny : RandomUnitData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitAny"/> class.
        /// </summary>
        public RandomUnitAny()
        {
        }

        /// <summary>
        /// Set to -1 for any level.
        /// </summary>
        public int Level { get; set; }

        public ItemClass Class { get; set; }

        public override string ToString() => $"Level {Level} ({Class})";
    }
}