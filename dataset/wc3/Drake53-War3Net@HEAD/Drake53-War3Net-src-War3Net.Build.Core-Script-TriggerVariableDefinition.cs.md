---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerVariableDefinition.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerVariableDefinition : TriggerItem
    {
        public TriggerVariableDefinition(TriggerItemType triggerItemType = TriggerItemType.Variable)
            : base(triggerItemType)
        {
        }
    }
}