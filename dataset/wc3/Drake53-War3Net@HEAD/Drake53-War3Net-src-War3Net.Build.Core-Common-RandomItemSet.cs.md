---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Common/RandomItemSet.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Common
{
    public sealed partial class RandomItemSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomItemSet"/> class.
        /// </summary>
        public RandomItemSet()
        {
        }

        public List<RandomItemSetItem> Items { get; init; } = new();
    }
}