---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/RandomUnitTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class RandomUnitTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomUnitTable"/> class.
        /// </summary>
        public RandomUnitTable()
        {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public List<WidgetType> Types { get; init; } = new();

        public List<RandomUnitSet> UnitSets { get; init; } = new();

        public override string ToString() => Name;
    }
}