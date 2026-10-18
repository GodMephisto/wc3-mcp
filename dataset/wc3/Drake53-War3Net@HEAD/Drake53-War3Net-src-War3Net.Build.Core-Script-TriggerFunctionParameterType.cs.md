---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerFunctionParameterType.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public enum TriggerFunctionParameterType
    {
        Preset = 0,
        Variable = 1,
        Function = 2,
        String = 3,

        Undefined = -1,
    }
}