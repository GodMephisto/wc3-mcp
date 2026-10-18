---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/SpecialDoodadData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class SpecialDoodadData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDoodadData"/> class.
        /// </summary>
        public SpecialDoodadData()
        {
        }

        public int TypeId { get; set; }

        public int Variation { get; set; }

        public Point Position { get; set; }

        public override string ToString() => TypeId.ToRawcode();
    }
}