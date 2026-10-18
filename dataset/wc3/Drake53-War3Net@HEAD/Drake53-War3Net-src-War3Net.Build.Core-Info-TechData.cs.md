---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/TechData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class TechData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TechData"/> class.
        /// </summary>
        public TechData()
        {
        }

        public Bitmask32 Players { get; set; }

        public int Id { get; set; }

        public override string ToString() => Id.ToRawcode();
    }
}