---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/RandomUnitCustomTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitCustomTable : RandomUnitData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitCustomTable"/> class.
        /// </summary>
        public RandomUnitCustomTable()
        {
        }

        public List<RandomUnitTableUnit> RandomUnits { get; init; } = new();
    }
}