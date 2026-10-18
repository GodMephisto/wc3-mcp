---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/RandomUnitGlobalTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class RandomUnitGlobalTable : RandomUnitData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitGlobalTable"/> class.
        /// </summary>
        public RandomUnitGlobalTable()
        {
        }

        public int TableId { get; set; }

        public int Column { get; set; }
    }
}