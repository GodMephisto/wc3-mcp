---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/ModifiedAbilityData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class ModifiedAbilityData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedAbilityData"/> class.
        /// </summary>
        public ModifiedAbilityData()
        {
        }

        public int AbilityId { get; set; }

        public bool IsAutocastActive { get; set; }

        public int HeroAbilityLevel { get; set; }

        public override string ToString() => AbilityId.ToRawcode();
    }
}