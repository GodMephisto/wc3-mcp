---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/RandomItemTable.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class RandomItemTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomItemTable"/> class.
        /// </summary>
        public RandomItemTable()
        {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public List<RandomItemSet> ItemSets { get; init; } = new();

        public override string ToString() => Name;
    }
}