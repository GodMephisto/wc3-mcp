---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Widget/RandomUnitData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public abstract partial class RandomUnitData
    {
        internal abstract void WriteTo(BinaryWriter writer, MapWidgetsFormatVersion formatVersion, MapWidgetsSubVersion subVersion, bool useNewFormat);
    }
}