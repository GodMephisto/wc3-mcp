---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/VariationObjectDataModification.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class VariationObjectDataModification : ObjectDataModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariationObjectDataModification"/> class.
        /// </summary>
        public VariationObjectDataModification()
        {
        }

        public int Variation { get; set; }

        public int Pointer { get; set; }
    }
}