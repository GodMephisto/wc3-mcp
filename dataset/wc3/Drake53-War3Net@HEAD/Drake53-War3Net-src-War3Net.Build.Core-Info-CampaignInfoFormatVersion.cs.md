---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/CampaignInfoFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Info
{
    /// <summary>
    /// File format version for <see cref="CampaignInfo"/>.
    /// </summary>
    public enum CampaignInfoFormatVersion
    {
        /// <summary>The initial version.</summary>
        v1 = 1,

        /// <summary>Introduced in patch 2.0.3.</summary>
        v2 = 2,

        /// <summary>Introduced in patch 3.0.0.</summary>
        v3 = 3,
    }
}