---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/ForceData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class ForceData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForceData"/> class.
        /// </summary>
        public ForceData()
        {
        }

        public ForceFlags Flags { get; set; }

        public Bitmask32 Players { get; set; }

        public string Name { get; set; }

        public override string ToString() => Name;
    }
}