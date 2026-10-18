---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/ObjectDataFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Object
{
    /// <summary>
    /// File format version for <see cref="ObjectData"/>, <see cref="AbilityObjectData"/>, <see cref="BuffObjectData"/>, <see cref="DestructableObjectData"/>,
    /// <see cref="DoodadObjectData"/>, <see cref="ItemObjectData"/>, <see cref="UnitObjectData"/>, and <see cref="UpgradeObjectData"/>.
    /// </summary>
    public enum ObjectDataFormatVersion
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        v1 = 1,

        /// <summary>The initial version.</summary>
        v2 = 2,

        /// <summary>Introduced in patch 1.33.</summary>
        v3 = 3,
    }
}