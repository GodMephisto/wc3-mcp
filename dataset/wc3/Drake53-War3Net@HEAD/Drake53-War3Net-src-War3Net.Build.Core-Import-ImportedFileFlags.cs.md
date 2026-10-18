---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Import/ImportedFileFlags.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Import
{
    [Flags]
    public enum ImportedFileFlags : byte
    {
        UNK1 = 1 << 0,
        UNK2 = 1 << 1,
        UNK4 = 1 << 2,
        UNK8 = 1 << 3,
        UNK16 = 1 << 4,
    }
}