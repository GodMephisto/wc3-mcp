---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Import/ImportedFilesFormatVersion.cs"
ref: HEAD
---

﻿#pragma warning disable CA1008
#pragma warning disable SA1300

namespace War3Net.Build.Import
{
    /// <summary>
    /// File format version for <see cref="ImportedFiles"/>.
    /// </summary>
    public enum ImportedFilesFormatVersion
    {
        /// <summary>The initial version.</summary>
        v1 = 1,
    }
}