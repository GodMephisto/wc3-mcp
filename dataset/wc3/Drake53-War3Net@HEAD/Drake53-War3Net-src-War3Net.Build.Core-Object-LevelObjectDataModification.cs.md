---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/LevelObjectDataModification.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class LevelObjectDataModification : ObjectDataModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelObjectDataModification"/> class.
        /// </summary>
        public LevelObjectDataModification()
        {
        }

        public int Level { get; set; }

        public int Pointer { get; set; }
    }
}