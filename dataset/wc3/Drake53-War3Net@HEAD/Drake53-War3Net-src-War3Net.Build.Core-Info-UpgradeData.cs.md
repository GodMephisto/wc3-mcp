---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/UpgradeData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class UpgradeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeData"/> class.
        /// </summary>
        public UpgradeData()
        {
        }

        public Bitmask32 Players { get; set; }

        public int Id { get; set; }

        // 0-indexed
        public int Level { get; set; }

        public UpgradeAvailability Availability { get; set; }

        public override string ToString() => Id.ToRawcode();
    }
}