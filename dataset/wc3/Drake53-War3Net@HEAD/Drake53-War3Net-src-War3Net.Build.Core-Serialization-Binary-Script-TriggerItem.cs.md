---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Script/TriggerItem.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public abstract partial class TriggerItem
    {
        internal abstract void WriteTo(BinaryWriter writer, MapTriggersFormatVersion formatVersion, MapTriggersSubVersion? subVersion);
    }
}